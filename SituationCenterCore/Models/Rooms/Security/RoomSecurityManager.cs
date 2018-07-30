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
using AutoMapper;
using SituationCenterCore.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Internal;

namespace SituationCenterCore.Models.Rooms.Security
{
    public class RoomSecurityManager : IRoomSecurityManager
    {
        private readonly ILogger<RoomSecurityManager> logger;
        private readonly IMapper mapper;
        private readonly IRoleAccessor roleAccessor;

        public RoomSecurityManager(
            ILogger<RoomSecurityManager> logger,
            IMapper mapper,
            IRoleAccessor roleAccessor)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.roleAccessor = roleAccessor;
        }

        public void CreateInvationRule(Room room, ICollection<ApplicationUser> users)
        {
            logger.LogDebug($"creating invation rule");
            if ((users?.Count ?? 0) == 0)
                throw new StatusCodeException(StatusCode.EmptyInvationRoom);
            var inviteRule = new RoomSecurityRule
            {
                PrivacyRule = PrivacyRoomType.InvationPrivate
            };
            foreach (var user in users)
            {
                user.UserRoomRoles.Add(new UserRoomRole
                {
                    UserId = user.Id,
                    RoleId = roleAccessor.InvitedId
                });
            }
            room.SecurityRule = inviteRule;
        }

        public void CreatePublicRule(Room room)
        {
            logger.LogDebug($"create public rule for room {room.Id} {room.Name}");

            var publicRule = new RoomSecurityRule { PrivacyRule = PrivacyRoomType.Public };
            room.SecurityRule = publicRule;
        }

        public void CreatePasswordRule(Room room, string password)
        {
            logger.LogDebug($"create password rule for room {room.Name}");

            if (password?.Length != 6 || !int.TryParse(password, out _))
                throw new StatusCodeException(StatusCode.IncorrectRoomPassword);

            var rule = new RoomSecurityRule()
            {
                PrivacyRule = PrivacyRoomType.Password,
                Password = password //TODO Использовать хеш!!!
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
                    ValidateInvation(room, user);
                    break;

                default:
                    throw new ApiArgumentException(paramName: nameof(rule));
            }
        }

        private void ValidatePassword(RoomSecurityRule securityRule, string password)
        {
            //TODO Использовать хэши!!!
            logger.LogDebug($"validating password");
            if (securityRule.Password != password)
                throw new StatusCodeException(StatusCode.IncorrectRoomPassword);
            logger.LogDebug($"success validated password");
        }

        private void ValidateInvation(Room room, ApplicationUser user)
        {
            logger.LogDebug("validating invite rule");
            if (room.UserRoomRoles.Any(urr => urr.UserId == user.Id))
                logger.LogDebug("success validated invite rule");
            else
                throw new StatusCodeException(StatusCode.AccessDenied);
        }

        public void AddAdminRole(ApplicationUser user, Room room)
        {
            user.UserRoomRoles = user.UserRoomRoles ?? new List<UserRoomRole>();
            user.UserRoomRoles.Add(new UserRoomRole
            {
                Room = room,
                RoleId = roleAccessor.AnministratorId
            });
        }

        public bool CanDelete(ApplicationUser user, Room room)
            => user
            .UserRoomRoles
            .Any(urr => urr.RoomId == room.Id && urr.RoleId == roleAccessor.AnministratorId);

        public bool CanJoin(ApplicationUser user, Room room)
        {
            if (room.SecurityRule.PrivacyRule != PrivacyRoomType.InvationPrivate)
                return true;
            try
            {
                ValidateInvation(room, user);
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
                .Where(r => r.SecurityRule.PrivacyRule != PrivacyRoomType.InvationPrivate
                       || r.UserRoomRoles.Any(role => role.RoleId == roleAccessor.InvitedId && role.UserId == userId));
        }
    }
}