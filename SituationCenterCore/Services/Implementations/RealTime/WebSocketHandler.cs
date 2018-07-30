using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SituationCenterCore.Services.Interfaces.RealTime;
using System.Text;
using Newtonsoft.Json;
using SituationCenterCore.Models.RealTime;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using SituationCenterCore.Data;

namespace SituationCenterCore.Services.Implementations.RealTime
{
    public class WebSocketHandler : IWebSocketHandler
    {
        private readonly IWebSocketManager webSocketManager;
        private readonly ILogger<WebSocketHandler> logger;
        private WebSocket webSocket;

        public event Action<string> TopicAdded;
        public event Action<string> TopicRemoved;
        public event Action<Guid> ConnectionLost;
        public Guid UserId { get; private set; }


        public WebSocketHandler(IWebSocketManager webSocketManager, ILogger<WebSocketHandler> logger)
        {
            this.webSocketManager = webSocketManager;
            this.logger = logger;
        }
        public async Task Handle(WebSocket webSocket, Guid userId)
        {
            UserId = userId;
            this.webSocket = webSocket;
            webSocketManager.Add(this);
            var buffer = new byte[1024 * 4];
            try 
            {
                while (webSocket.State != WebSocketState.Closed)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        ConnectionLost?.Invoke(userId);
                        break;
                    }
                    var stringMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    logger.LogDebug($"user {userId} send {stringMessage}");
                    var messageType = JsonConvert.DeserializeObject<Message>(stringMessage).MessageType;
                    switch (messageType)
                    {
                        case MessageType.AddTopic:
                            var addTopic = To<string>(stringMessage);
                            TopicAdded?.Invoke(addTopic.Data);
                            break;
                        case MessageType.RemoveTopic:
                            var remTopic = To<string>(stringMessage);
                            TopicRemoved?.Invoke(remTopic.Data);
                            break;
                        default:
                            logger.LogDebug($"incorrect message {stringMessage}");
                            break;
                    }

                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType,
                        result.EndOfMessage, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "error while web socket connect");
                ConnectionLost?.Invoke(userId);
            }
        }
        public async Task Send(string topic, object data)
        {
            try
            {
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))),
                                       WebSocketMessageType.Text,
                                       true,
                                       CancellationToken.None);
            }
            catch (Exception e)
            {
                logger.LogInformation(e, "Error while sending");
                ConnectionLost?.Invoke(UserId);
            }
        }

        private static GenericMessage<T> To<T>(string message)
            => JsonConvert.DeserializeObject<GenericMessage<T>>(message);
    }
}
