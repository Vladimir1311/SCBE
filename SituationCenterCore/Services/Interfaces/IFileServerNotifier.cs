using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Services.Interfaces
{
    public interface IFileServerNotifier
    {
        Task AddToken(Guid userId, string token);
        Task SetRoom(Guid userId, Guid? roomId);
    }
}
