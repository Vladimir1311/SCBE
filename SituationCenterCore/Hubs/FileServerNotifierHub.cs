using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SituationCenterCore.Services.Interfaces;

namespace SituationCenterCore.Hubs
{
    public class FileServerNotifierHub : Hub
    {
        private readonly ISharedUsersState sharedUsersState;

        public FileServerNotifierHub(ISharedUsersState sharedUsersState)
        {
            this.sharedUsersState = sharedUsersState;
            sharedUsersState.TokenCreated += async (uId, token ) => await AddToken(uId, token);
            sharedUsersState.RoomChanged += async (uId, rId ) => await SetRoom(uId, rId);
        }

        public Task Test(string param)
        {
            return Clients.All.SendAsync("token", Guid.NewGuid(), "some string");
        }

        private Task AddToken(Guid userId, string token)
        {
            return Clients.All.SendAsync("token", userId, token );
        }

        private Task SetRoom(Guid userId, Guid? roomId)
        {
            return Clients.All.SendAsync("roomId", userId, roomId );
        }
    }
}
