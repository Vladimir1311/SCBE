using CCF.Shared;
using IPResolver.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace IPResolver.Models
{
    public class RemotePoint
    {
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private HashSet<(Guid id, DateTime sendedTime)> pingPairs = new HashSet<(Guid, DateTime)>();
       
        public DateTime ConnectionTime { get; set; }
        public DateTime LastPing { get; private set; }
        public TimeSpan PingTime { get; private set; }
        public string Password { get; set; }
        public string InterfaceName { get; set; }

        public TcpClient Connection { get; set; }

        public async Task SendMessage(long packLength, Guid packId, MessageType type, Stream from)
        {
            var stream = Connection.GetStream();
            await semaphoreSlim.WaitAsync();
            try
            {
                stream.Write(BitConverter.GetBytes(packLength));
                stream.Write(packId.ToByteArray());
                stream.WriteByte((byte)type);
                if (packLength - 17 == 0)
                    return;
                await from?.CopyPart(stream, (int)packLength - 17);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        public async Task SendPing()
        {
            var pingPair = (id: Guid.NewGuid(), DateTime.Now);
            pingPairs.Add(pingPair);
            await SendMessage(17, pingPair.id, MessageType.PingRequest, null);
        }


        public void SetPing(Guid pingId)
        {

            try
            {
                var pair = pingPairs.First(P => P.id == pingId);
                var now = DateTime.Now;
                LastPing = now;
                PingTime = now - pair.sendedTime;
                pingPairs.Remove(pair);
            }
            catch { return; }

        }
    }
}