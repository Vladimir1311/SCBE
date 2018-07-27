using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SituationCenterCore.Services.Interfaces;

namespace SituationCenterCore.Services.Implementations
{
    public class InMemorySharedUsersState : ISharedUsersState
    {
        public event Action<Guid, string> TokenCreated;
        public event Action<Guid, Guid?> RoomChanged;

        public Task AddToken(Guid userId, string token)
        {
            TokenCreated?.Invoke(userId, token);
            return Task.CompletedTask;
        }

        public Task SetRoom(Guid userId, Guid? roomId)
        {
            RoomChanged?.Invoke(userId, roomId);
            return Task.CompletedTask;
        }

    }
}
