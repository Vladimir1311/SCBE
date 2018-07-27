using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Services.Interfaces
{
    public interface ISharedUsersState
    {
        Task AddToken(Guid userId, string token);
        Task SetRoom(Guid userId, Guid? roomId);

        event Action<Guid, string> TokenCreated;
        event Action<Guid, Guid?> RoomChanged;
    }
}
