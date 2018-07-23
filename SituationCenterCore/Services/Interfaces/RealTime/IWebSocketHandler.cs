using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using SituationCenterCore.Data;

namespace SituationCenterCore.Services.Interfaces.RealTime
{
    public interface IWebSocketHandler
    {
        event Action<string> TopicAdded;
        event Action<string> TopicRemoved;

        Task Handle(WebSocket webSocket, Guid userId);
        Task Send(string topic, object data);
        Guid UserId { get; }
    }
}
