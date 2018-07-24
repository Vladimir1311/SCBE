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

        public void CreateInvationRule(Room room, ICollection<Guid> usersIds)
        {
            logger.LogDebug($"creating invation rule");
            if ((usersIds?.Count ?? 0) == 0)
                throw new StatusCodeException(StatusCode.EmptyInvationRoom);
            var inviteRule = new RoomSecurityRule
            {
                PrivacyRule = PrivacyRoomType.InvationPrivate,
                Invites = usersIds.Select(g => mapper.Map<UserRoomInvite>(g)).ToList()
            };

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
            if (securityRule.Password != password)
                throw new StatusCodeException(StatusCode.IncorrectRoomPassword);
            logger.LogDebug($"success validated password");
        }

        private void ValidateInvation(RoomSecurityRule rule, ApplicationUser user)
        {
            logger.LogDebug("validating invite rule");
            if (rule.Invites.Any(uri => uri.UserId == user.Id))
                logger.LogDebug("success validated invite rule");
            else
                throw new StatusCodeException(StatusCode.AccessDenied);
        }

        public void AddAdminRole(ApplicationUser user, Room room)
        {
            user.UserRoomRole = new UserRoomRole
            {
                Room = room,
                RoleId = roleAccessor.AnministratorId
            };
        }

        public bool CanDelete(ApplicationUser user, Room room)
            => user.UserRoomRole.RoleId == roleAccessor.AnministratorId;

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
                .Concat(rooms.Where(r => r.SecurityRule.PrivacyRule != PrivacyRoomType.InvationPrivate))
                .Concat(rooms.Where(r => r.Users.Any(u => u.Id == userId)));
        }
    }
}