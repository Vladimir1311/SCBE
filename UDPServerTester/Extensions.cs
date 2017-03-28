using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPServerTester
{
    public static class Extensions
    {
        public static async Task SendAsync(this WebSocket socket, string message)
        {
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task<string> GetMessage(this WebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[4096]);
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            return Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
        }
    }
}
