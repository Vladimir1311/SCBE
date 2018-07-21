﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SituationCenterCore.Models.Rooms.Security;

namespace SituationCenterCore.Data.DatabaseAbstraction
{
    public class EntityRepository : IRepository
    {
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;

        IQueryable<ApplicationUser> IRepository.Users => userManager.Users;

        public EntityRepository(
            DbContextOptions<ApplicationDbContext> options, 
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.dbContext = dbContext;
        }
        public async Task<RoomSecurityRule[]> GetRulesAsync() =>
            await Task.FromResult(dbContext.Rules.ToArray());

        public async Task<IdentityResult> CreateRoleAsync(IdentityRole<Guid> role) =>
            await roleManager.CreateAsync(role);

        public async Task<IdentityRole<Guid>> FindRoleByNameAsync(string name) =>
            await roleManager.FindByNameAsync(name);

        public async Task<IdentityResult> DeleteRoleAsync(IdentityRole<Guid> identityRole) =>
            await roleManager.DeleteAsync(identityRole);

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName) =>
            await userManager.IsInRoleAsync(user, roleName);
        

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName) =>
            await userManager.AddToRoleAsync(user, roleName);


        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password) =>
            await userManager.CreateAsync(user, password);

        public Task<List<ApplicationUser>> FindUsers(Func<ApplicationUser, bool> predicate) =>
            Task.FromResult(dbContext.Users.Where(predicate).ToList());

        public Task<bool> AnyUser(Func<ApplicationUser, bool> predicate) =>
            Task.FromResult(dbContext.Users.Any(predicate));

        public Task<ApplicationUser> FindUserByEmailAsync(string email) =>
            userManager.FindByEmailAsync(email);

        public Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password) =>
            userManager.CheckPasswordAsync(user, password);

        public Task<ApplicationUser> FindUser(ClaimsPrincipal user) =>
            userManager.FindByIdAsync(userManager.GetUserId(user));

        public Guid GetUserId(ClaimsPrincipal user) =>
            Guid.Parse(userManager.GetUserId(user));

        public async Task<RoomSecurityRule> GetRuleAsync(Guid ruleId) =>
            await dbContext.Rules.FirstOrDefaultAsync(R => R.Id == ruleId);
    }
}
