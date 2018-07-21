using Microsoft.AspNetCore.Identity;
using SituationCenterCore.Models.Rooms.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SituationCenterCore.Data.DatabaseAbstraction
{
    public interface IRepository
    {
        Task<RoomSecurityRule[]> GetRulesAsync();
        Task<RoomSecurityRule> GetRuleAsync(Guid ruleId);
        Task<IdentityResult> CreateRoleAsync(IdentityRole<Guid> role);
        Task<IdentityRole<Guid>> FindRoleByNameAsync(string name);
        Task<IdentityResult> DeleteRoleAsync(IdentityRole<Guid> identityRole);

        Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);

        Task<List<ApplicationUser>> FindUsers(Func<ApplicationUser, bool> predicate);
        Task<bool> AnyUser(Func<ApplicationUser, bool> predicate);
        Task<ApplicationUser> FindUserByEmailAsync(string email);
        Task<ApplicationUser> FindUser(ClaimsPrincipal user);
        Guid GetUserId(ClaimsPrincipal user);
        Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password);
        IQueryable<ApplicationUser> Users { get; }

    }
}
