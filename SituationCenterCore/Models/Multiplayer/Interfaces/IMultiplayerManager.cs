using SituationCenterCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.Multiplayer.Interfaces
{
    public interface IMultiplayerManager
    {
        Task AddClient(WebSocket webSocket, ApplicationUser userId);
    }
}
