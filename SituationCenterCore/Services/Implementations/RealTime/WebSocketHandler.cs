using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SituationCenterCore.Services.Interfaces.RealTime;

namespace SituationCenterCore.Services.Implementations.RealTime
{
    public class WebSocketHandler : IWebSocketHandler
    {
        private readonly IWebSocketManager webSocketManager;
        private readonly ILogger<WebSocketHandler> logger;

        public WebSocketHandler(IWebSocketManager webSocketManager, ILogger<WebSocketHandler> logger)
        {
            this.webSocketManager = webSocketManager;
            this.logger = logger;
        }
        public async Task Handle(WebSocket webSocket)
        {
            webSocketManager.Add(webSocket);
            var buffer = new byte[1024 * 4];
            try
            {
                while (webSocket.State != WebSocketState.Closed)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType,
                        result.EndOfMessage, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "error while web socket connect");
            }
        }
    }
}
