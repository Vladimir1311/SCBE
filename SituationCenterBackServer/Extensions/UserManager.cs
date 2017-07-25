using Microsoft.AspNetCore.Identity;
using SituationCenterBackServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Extensions
{
    public static class UserManager
    {
        public static async Task<ApplicationUser> FindUser(this UserManager<ApplicationUser> userManager, ClaimsPrincipal user)
        {
            return await userManager.FindByIdAsync(userManager.GetUserId(user));
        }

        public static Guid GetUserGuid(this UserManager<ApplicationUser> userManager, ClaimsPrincipal user)
        {
            return Guid.Parse(userManager.GetUserId(user));
        }
    }
}
