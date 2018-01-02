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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPResolver.Models.Points
{
    public class RemotePoint : IDisposable
    {
        private SemaphoreSlim sendingMessageSemaphore = new SemaphoreSlim(1, 1);

        public DateTime ConnectionTime { get; set; }
        public Pinger Pinger { get; }
        public string Password { get; set; }

        private TcpClient tcpClient;
        private BinaryReader reader;

        public RemotePoint(TcpClient client, ILoggerFactory factory)
        {
            Pinger = new Pinger(SendMessage, factory.CreateLogger<Pinger>());
            tcpClient = client;
            reader = new BinaryReader(tcpClient.GetStream(), Encoding.UTF8, true);
        }

        public string IPAddress => (tcpClient.Client.RemoteEndPoint as IPEndPoint)?.Address.MapToIPv4().ToString()
            ?? "error while getting ip";

        public async Task SendMessage(long packLength, Guid packId, MessageType type, Stream from)
        {
            var stream = tcpClient.GetStream();
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

        public (long length, Guid packId, MessageType messageType, Stream data) ReadMessage()
        {
            long length = reader.ReadInt64();
            Guid packId = new Guid(reader.ReadBytes(16));
            MessageType type = (MessageType)reader.ReadByte();
            if (type == MessageType.PingResponse)
            {
                Pinger.SetPing(packId);
                return ReadMessage();
            }
            return (length, packId, type, reader.BaseStream.Partial(length));
        }


        public void Dispose()
        {
            tcpClient.Dispose();
        }
    }

}
