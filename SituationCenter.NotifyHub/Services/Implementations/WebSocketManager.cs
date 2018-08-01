using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using SituationCenter.NotifyHub.Services.Interfaces;

namespace SituationCenter.NotifyHub.Services.Implementations
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly ConcurrentDictionary<Guid, IWebSocketHandler> sockets =
            new ConcurrentDictionary<Guid, IWebSocketHandler>();

        private readonly List<(string topic, Guid userId)> subscriptions =
            new List<(string, Guid)>();

        public WebSocketManager(IApplicationLifetime lifetime)
        {
            lifetime.ApplicationStopping.Register(Dispose);
        }

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
                    .ToList();
        }

        public void Dispose()
        {
            foreach (var socketHandler in sockets.Values)
            {
                socketHandler.Dispose();
            }
        }
    }
}
