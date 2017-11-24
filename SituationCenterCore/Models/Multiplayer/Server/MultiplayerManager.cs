
using SituationCenterCore.Models.Multiplayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Text;

namespace SituationCenterCore.Models.Multiplayer.Server
{
    public class MultiplayerManager : IMultiplayerManager
    {
        private List<WebSocket> endPoints = new List<WebSocket>();
        
        public async Task AddClient(WebSocket webSocket, string userId)
        {
            endPoints.Add(webSocket);
            while (true)
            {
                var message = await ReadMessageFrom(webSocket);
                await webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async static Task<byte[]> ReadMessageFrom(WebSocket socket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return buffer;

        }

        
    }
}
