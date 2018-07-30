using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using SituationCenterCore.Services.Interfaces.RealTime;
using SituationCenterCore.Extensions;

namespace SituationCenterCore.Services.Implementations.RealTime
{
    public class WebSocketManager : IWebSocketManager
    {
        private ConcurrentDictionary<Guid, IWebSocketHandler> sockets =
            new ConcurrentDictionary<Guid, IWebSocketHandler>();

        private List<(string topic, Guid userId)> subscriptions =
            new List<(string, Guid)>();
        public void Add(IWebSocketHandler webSocketHandler)
        {
            sockets[webSocketHandler.UserId] = webSocketHandler;

            webSocketHandler.TopicAdded += topic =>
            {
                subscriptions.Add((topic, webSocketHandler.UserId));
            };

            webSocketHandler.TopicRemoved += topic =>
            {
                subscriptions.RemoveAll(subscr => subscr.topic == topic && subscr.userId == webSocketHandler.UserId);
            };

            webSocketHandler.ConnectionLost += userId =>
            {
                subscriptions.RemoveAll(sub => sub.userId == userId);
                sockets.TryRemove(userId, out _);
            };
        }
        public IEnumerable<IWebSocketHandler> ForTopic(string topic)
        {
            return
                subscriptions
                    .Where(sub => sub.topic == topic)
                    .Select(sub => sub.userId)
                    .Select(sockets.GetValueOrDefault)
                    //.DefaultIfEmpty()
                    .ToList();
        }

    }
}
