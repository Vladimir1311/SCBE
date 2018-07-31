using System;
using System.Collections.Generic;

namespace SituationCenter.NotifyHub.Services.Interfaces
{
    public interface IWebSocketManager : IDisposable
    {
        void Add(IWebSocketHandler webSocketHandler);
        IEnumerable<IWebSocketHandler> ForTopic(string topic);
    }
}
