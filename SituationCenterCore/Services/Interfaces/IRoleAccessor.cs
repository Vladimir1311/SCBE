using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SituationCenterCore.Data;

namespace SituationCenterCore.Services.Interfaces
{
    public interface IRoleAccessor
    {
        Guid AnministratorId { get; }
        void SetDbContext(ApplicationDbContext dbContext);
    }
}
