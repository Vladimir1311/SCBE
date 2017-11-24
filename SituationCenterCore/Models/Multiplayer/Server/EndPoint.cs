using Newtonsoft.Json;
using SituationCenterCore.Data;
using SituationCenterCore.Models.Multiplayer.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.Multiplayer.Server
{
    public class EndPoint
    {
        private static int count;
        private int id;
        public EndPoint(WebSocket connection, ApplicationUser user)
        {
            id = count++;
            Connection = connection;
            User = user;
        }
        private SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
        public WebSocket Connection { get; set; }
        public ApplicationUser User { get; set; }

        public async Task SendMessageAsync(BaseMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            await SendTextAsync(json);
        }
        public async Task SendTextAsync(string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            await SendBytesAsync(bytes);
        }
        public async Task SendBytesAsync(byte[] bytes)
        {
            await semaphore.WaitAsync();
            await Connection.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            semaphore.Release();
        }
        
        public async Task<string> ReadMessageAsync()
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await Connection.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }
        public override bool Equals(object obj)
        {
            var point = obj as EndPoint;
            return point != null &&
                   id == point.id;
        }

        public override int GetHashCode()
        {
            return 1877310944 ^ id.GetHashCode();
        }
    }
}
