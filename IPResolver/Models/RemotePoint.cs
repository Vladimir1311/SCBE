using CCF.Shared;
using IPResolver.Extensions;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IPResolver.Models
{
    public class RemotePoint
    {
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public DateTime ConnectionTime { get; set; }
        public DateTime LastPing { get; set; }
        public string Password { get; set; }
        public string InterfaceName { get; set; }

        public TcpClient Connection { get; set; }
        public async Task SendMessage(int packLength, Guid packId, MessageType type, Stream from)
        {
            var stream = Connection.GetStream();
            await semaphoreSlim.WaitAsync();
            try
            {
                stream.Write(BitConverter.GetBytes(packLength));
                stream.Write(packId.ToByteArray());
                stream.Write(new byte[] { (byte)type });
                from.CopyPart(stream, packLength - 17).Wait();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        public async Task SendPing(Guid messageId) => await SendMessage(17, messageId, MessageType.PingRequest, null);
    }
}