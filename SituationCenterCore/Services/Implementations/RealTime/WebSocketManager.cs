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

        private ConcurrentDictionary<string, List<Guid>> topics = 
            new ConcurrentDictionary<string, List<Guid>>();
        public void Add(IWebSocketHandler webSocketHandler)
        {
            sockets[webSocketHandler.UserId] = webSocketHandler;
            webSocketHandler.TopicAdded += topic 
                => topics.AddOrUpdate(topic, new List<Guid>{webSocketHandler.UserId}, 
                                      (t, list) => list.With(l => l.Add(webSocketHandler.UserId)));

            webSocketHandler.TopicRemoved += topic
                => topics.GetValueOrDefault(topic)?.Remove(webSocketHandler.UserId);
        }
        public IEnumerable<IWebSocketHandler> ForTopic(string topic)
        {
            var success = topics.TryGetValue(topic, out var guids);
            if (!success)
                return Enumerable.Empty<IWebSocketHandler>();
            var all = guids.Select(sockets.GetValueOrDefault);
            return all;
        }

    }
}
