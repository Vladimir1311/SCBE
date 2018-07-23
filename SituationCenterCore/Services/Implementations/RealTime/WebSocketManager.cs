using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using SituationCenterCore.Services.Interfaces.RealTime;

namespace SituationCenterCore.Services.Implementations.RealTime
{
    public class WebSocketManager : IWebSocketManager
    {
        private ConcurrentDictionary<Guid, WebSocket> sockets = new ConcurrentDictionary<Guid, WebSocket>();
        public void Add(WebSocket webSocket)
        {
            sockets[Guid.NewGuid()] = webSocket;
        }
    }
}
