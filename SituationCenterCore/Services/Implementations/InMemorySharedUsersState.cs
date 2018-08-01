using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SituationCenter.NotifyProtocol;
using SituationCenterCore.Services.Interfaces;

namespace SituationCenterCore.Services.Implementations
{
    public class InMemorySharedUsersState : ISharedUsersState
    {
        private readonly INotificator notificator;
        public event Action<Guid, string> TokenCreated;
        public event Action<Guid, Guid?> RoomChanged;

        public InMemorySharedUsersState(INotificator notificator)
        {
            this.notificator = notificator;
        }

        public Task AddToken(Guid userId, string token)
        {
            TokenCreated?.Invoke(userId, token);
            return notificator.Notify("new_token", new {userId, token});
        }

        public Task SetRoom(Guid userId, Guid? roomId)
        {
            RoomChanged?.Invoke(userId, roomId);
            return notificator.Notify("current_room", new {userId, roomId});
        }

    }
}
