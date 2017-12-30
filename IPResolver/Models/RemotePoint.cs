using CCF.Shared;
using IPResolver.Extensions;
using IPResolver.Models.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IPResolver.Models
{
    public class RemotePoint : IDisposable
    {
        private SemaphoreSlim sendingMessageSemaphore = new SemaphoreSlim(1, 1);

        public DateTime ConnectionTime { get; set; }
        public Pinger Pinger { get; }
        public string Password { get; set; }
        public string SelfInterfaceName { get; set; }
        public string WantedInterfaceName { get; set; }

        private TcpClient connection { get; set; }
        private HashSet<CreateInstanceSet> instanceCreating =new HashSet<CreateInstanceSet>();

        public RemotePoint(ILoggerFactory factory)
        {
            Pinger = new Pinger(SendMessage, factory.CreateLogger<Pinger>());
        }

        public string IPAddress => (connection.Client.RemoteEndPoint as IPEndPoint)?.Address.MapToIPv4().ToString()
            ?? "error while getting ip";

        public async Task SendMessage(long packLength, Guid packId, MessageType type, Stream from)
        {
            var stream = connection.GetStream();
            await sendingMessageSemaphore.WaitAsync();
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
                sendingMessageSemaphore.Release();
            }
        }

        public async Task<int> CreateInstanse()
        {
            if (SelfInterfaceName == null)
                throw new Exception("Try to create instance on only client EndPoint");
            var semaphore = new SemaphoreSlim(1, 1);
            var set = new CreateInstanceSet { PackId = Guid.NewGuid(), ServiceId = -1, MsEvent = semaphore };
            instanceCreating.Add(set);
            await SendMessage(17, set.PackId, MessageType.ServiceCreateRequest, null);
            await semaphore.WaitAsync();
            return set.ServiceId;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        private class CreateInstanceSet
        {
            public Guid PackId { get; set; }
            public int ServiceId { get; set; }
            public SemaphoreSlim MsEvent { get; set; }
        }

        internal void InitConnection(TcpClient client)
        {
            connection = client;
        }
    }

}
