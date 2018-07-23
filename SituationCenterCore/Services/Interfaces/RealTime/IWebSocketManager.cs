using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SituationCenterCore.Services.Interfaces.RealTime
{
    public interface IWebSocketManager
    {
        void Add(WebSocket webSocket);
    }
}
