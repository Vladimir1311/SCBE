using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SituationCenter.Shared.Exceptions;
using SituationCenter.Shared.Models.Rooms;
using SituationCenterCore.Data;
using SituationCenterCore.Data.DatabaseAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.Rooms.Security
{
    public class RoomSecurityManager : IRoomSecurityManager
    {
        private readonly IRepository repository;
        private readonly ILogger<RoomSecurityManager> logger;

        //TODO Add rules generator in DI
        private RoomRolesGenerator roomRolesGenerator = new RoomRolesGenerator();

        public RoomSecurityManager(
            IRepository repository,
            ILogger<RoomSecurityManager> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public void CreateInvationRule(Room room, Guid[] usersIds)
        {
            logger.LogDebug($"creating invation rule");
            if ((usersIds?.Length ?? 0) == 0)
                throw new StatusCodeException(StatusCode.EmptyInvationRoom);
            var inviteRule = new RoomSecurityRule() { PrivacyRule = PrivacyRoomType.InvationPrivate };

            var userIdsString = string.Join("\n", usersIds.Select(I => I.ToString()));
            inviteRule.Data = userIdsString;

            room.SecurityRule = inviteRule;
        }

        public void CreatePublicRule(Room room)
        {
            logger.LogDebug($"create public rule for room {room.Id} {room.Name}");

            var publicRule = new RoomSecurityRule() { PrivacyRule = PrivacyRoomType.Public };
            room.SecurityRule = publicRule;
        }

        public void CreatePasswordRule(Room room, string password)
        {
            logger.LogDebug($"create password rule for room {room.Name}");

            if (password?.Length != 6 || !int.TryParse(password, out _))
                throw new StatusCodeException(StatusCode.IncorrectRoomPassword);
            
            RoomSecurityRule rule = new RoomSecurityRule()
            {
                PrivacyRule = PrivacyRoomType.Password,
                Data = password //TODO Использовать хеш!!!
            };
            room.SecurityRule = rule;
        }

        public void Validate(ApplicationUser user, Room room, string data)
        {
            logger.LogDebug($"user {user.Email} try to join room {room.Id}, with {data}");
            var rule = room.SecurityRule;
            logger.LogDebug($"room {room.Id} have privacy rule {rule.PrivacyRule}");
            switch (rule.PrivacyRule)
            {
                case PrivacyRoomType.Public:
                    break;

                case PrivacyRoomType.Password:
                    ValidatePassword(rule, data);
                    break;

                case PrivacyRoomType.InvationPrivate:
                    ValidateInvation(rule, user);
                    break;

                default:
                    throw new ApiArgumentException(paramName: nameof(rule));
            }
        }

        private void ValidatePassword(RoomSecurityRule securityRule, string password)
        {
            //TODO Использовать хэши!!!
            logger.LogDebug($"validating password");
            if (securityRule.Data != password)
                throw new StatusCodeException(StatusCode.IncorrectRoomPassword);
            logger.LogDebug($"success validated password");
        }

        private void ValidateInvation(RoomSecurityRule rule, ApplicationUser user)
        {
            logger.LogDebug("validating invite rule");
            if (!rule.Data.Split('\n').Contains(user.Id.ToString()))
                throw new StatusCodeException(StatusCode.AccessDenied);
            logger.LogDebug("success validated invite rule");
        }

        public async Task AddAdminRole(ApplicationUser user, Room room)
        {
            var adminRole = roomRolesGenerator.GetAdministratorRole(room);
            var createResult = await repository.CreateRoleAsync(new IdentityRole<Guid>(adminRole));
            if (!createResult.Succeeded)
                throw new Exception("Can't create role for room " + string.Join(" ", createResult.Errors.Select(E => $"{E.Code} {E.Description}")));
            var addToRoleResult = await repository.AddToRoleAsync(user, adminRole);
            if (!addToRoleResult.Succeeded)
                throw new Exception("Can't add to room" + string.Join(" ", addToRoleResult.Errors.Select(E => $"{E.Code} {E.Description}")));
        }

        public bool CanDelete(ApplicationUser user, Room room)
        {
            var adminRole = roomRolesGenerator.GetAdministratorRole(room);
            return repository.IsInRoleAsync(user, adminRole).Result
                || repository.IsInRoleAsync(user, "Administrator").Result;
        }

        public void ClearRoles(Room room)
        {
            var adminRoleName = roomRolesGenerator.GetAdministratorRole(room);
            var adminRole = repository.FindRoleByNameAsync(adminRoleName).Result;
            if (adminRole != null)
                repository.DeleteRoleAsync(adminRole).Wait();
        }

        public bool CanJoin(ApplicationUser user, Room room)
        {
            if (room.SecurityRule.PrivacyRule != PrivacyRoomType.InvationPrivate)
                return true;
            try
            {
                ValidateInvation(room.SecurityRule, user);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public IQueryable<Room> AccessedRooms(IQueryable<Room> rooms, Guid userId)
        {
            return rooms
                .Where(r => r.SecurityRule.PrivacyRule == PrivacyRoomType.InvationPrivate)
                .Where(r => r.SecurityRule.Invites.Any(inv => inv.UserId == userId))
                .Concat(rooms.Where(r => r.SecurityRule.PrivacyRule != PrivacyRoomType.InvationPrivate));
        }
    }
}