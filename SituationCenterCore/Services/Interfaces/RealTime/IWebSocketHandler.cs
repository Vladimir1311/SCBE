using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SituationCenterCore.Services.Interfaces.RealTime
{
    interface IWebSocketHandler
    {
        Task Handle(WebSocket webSocket);
    }
}
