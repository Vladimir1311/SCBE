using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SituationCenter.NotifyHub.Services.Interfaces
{
    public interface IWebSocketHandler : IDisposable
    {
        event Action<string> TopicAdded;
        event Action<string> TopicRemoved;
        event Action<Guid> ConnectionLost;

        Task Handle(WebSocket webSocket, Guid userId);
        Task Send<T>(string topic, T data);
        Guid UserId { get; }
    }
}
