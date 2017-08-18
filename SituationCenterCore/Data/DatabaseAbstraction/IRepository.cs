using Microsoft.AspNetCore.Identity;
using SituationCenterCore.Models.Rooms.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Data.DatabaseAbstraction
{
    public interface IRepository
    {
        Task<RoomSecurityRule[]> GetRulesAsync();
        Task<IdentityResult> CreateRoleAsync(IdentityRole role);
        Task<IdentityRole> FindRoleByNameAsync(string name);
        Task<IdentityResult> DeleteRoleAsync(IdentityRole identityRole);

        Task<bool> IsInRoleAsync(ApplicationUser user, string roleName);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);

        Task<List<ApplicationUser>> FindUsers(Func<ApplicationUser, bool> predicate);
        Task<bool> AnyUser(Func<ApplicationUser, bool> predicate);
        Task<ApplicationUser> FindUserByEmailAsync(string email);
        Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password);


    }
}
