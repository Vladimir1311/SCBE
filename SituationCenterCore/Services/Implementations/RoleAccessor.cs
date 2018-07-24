using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SituationCenterCore.Data;
using SituationCenterCore.Services.Interfaces;

namespace SituationCenterCore.Services.Implementations
{
    public class RoleAccessor : IRoleAccessor
    {
        private readonly IServiceProvider serviceProvider;
        private const string AdministratorRoleName = "Administrator";
        private ApplicationDbContext dbContext;

        public RoleAccessor(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private static Guid? adminId;
        public Guid AnministratorId => adminId ?? (adminId = GetRoleId(AdministratorRoleName)).Value;
        public void SetDbContext(ApplicationDbContext context)
        {
            this.dbContext = context;
        }

        private Guid GetRoleId(string roleName)
        {
            
            var inDb = dbContext.Roles.SingleOrDefaultAsync(r => r.Name == roleName).Result?.Id;
            return inDb ?? CreateRole(AdministratorRoleName, dbContext);
        }

        private Guid CreateRole(string roleName, ApplicationDbContext dbContext)
        {
            var role = new Role(roleName);
            dbContext.Roles.Add(role);
            dbContext.SaveChanges();
            return role.Id;
        }
    }
}
