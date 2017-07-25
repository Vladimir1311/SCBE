using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SituationCenterBackServer.Models.VoiceChatModels;
using SituationCenterBackServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace SituationCenterBackServer.Models.RoomSecurity
{
    public class RoomSecurityManager : IRoomSecurityManager
    {
        private readonly ApplicationDbContext dataBase;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RoomSecurityManager> logger;

        //TODO Add rules generator in DI
        private RoomRolesGenerator roomRolesGenerator = new RoomRolesGenerator();


        public RoomSecurityManager(ApplicationDbContext dataBase,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RoomSecurityManager> logger)
        {
            this.dataBase = dataBase;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        public void CreateInvationRule(Room room, IEnumerable<ApplicationUser> users)
        {
            throw new NotImplementedException();
        }


        public void CreatePublicRule(Room room)
        {
            logger.LogDebug($"create public rule for room {room.Id} {room.Name}");

            var publicRule = new RoomSecurityRule() { PrivacyRule = Common.Models.Rooms.PrivacyRoomType.Public };
            room.SecurityRule = publicRule;
            
        }

        public void CreatePasswordRule(Room room, string password)
        {
            logger.LogDebug($"create password rule for room {room.Id} {room.Name}");
            RoomSecurityRule rule = new RoomSecurityRule()
            {
                PrivacyRule = Common.Models.Rooms.PrivacyRoomType.Password,
                Data = password
            };
            room.SecurityRule = rule;
        }

        public void Validate(ApplicationUser user, Room room, string data)
        {
            logger.LogDebug($"user {user.Email} try to join room {room.Id}, with {data}");
            var rule = room.SecurityRule ?? dataBase.Rules.FirstOrDefault(R => R.Id == room.RoomSecurityRuleId);
            logger.LogDebug($"room {room.Id} have privacy rule {rule.PrivacyRule}");
            switch (rule.PrivacyRule)
            {
                case Common.Models.Rooms.PrivacyRoomType.Public:
                    break;
                case Common.Models.Rooms.PrivacyRoomType.Password:
                    ValidatePassword(rule, data);
                    break;
                case Common.Models.Rooms.PrivacyRoomType.InvationPrivate:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new Exception("Unknown privacy type");
            }
        }

        private void ValidatePassword(RoomSecurityRule securityRule, string password)
        {
            logger.LogDebug($"validate password");
            if (securityRule.Data != password)
                throw new Exception("Password not correct");
        }


        public void AddAdminRole(ApplicationUser user, Room room)
        {
            var adminRole = roomRolesGenerator.GetAdministratorRole(room);
            var createResult = roleManager.CreateAsync(new IdentityRole(adminRole)).Result;
            if (!createResult.Succeeded)
                throw new Exception("Can't create role for room " + string.Join(" ", createResult.Errors.Select(E => $"{E.Code} {E.Description}")));
            var addToRoleResult = userManager.AddToRoleAsync(user, adminRole).Result;
            if (!addToRoleResult.Succeeded)
                throw new Exception("Can't add to room" + string.Join(" ", addToRoleResult.Errors.Select(E => $"{E.Code} {E.Description}")));
        }

        public bool CanDelete(ApplicationUser user, Room room)
        {
            var adminRole = roomRolesGenerator.GetAdministratorRole(room);
            return userManager.IsInRoleAsync(user, adminRole).Result
                || userManager.IsInRoleAsync(user, "Administrator").Result
                ;
        }

        public void ClearRoles(Room room)
        {
            var adminRoleName = roomRolesGenerator.GetAdministratorRole(room);
            var adminRole = roleManager.FindByNameAsync(adminRoleName).Result;
            if (adminRole != null)
                roleManager.DeleteAsync(adminRole).Wait();
        }
    }
}
