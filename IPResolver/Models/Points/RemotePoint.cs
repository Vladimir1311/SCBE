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
        private SemaphoreSlim readindMessageSemaphore = new SemaphoreSlim(1, 1);

        public DateTime ConnectionTime { get; set; }
        public Pinger Pinger { get; }
        public string Password { get; set; }
        public event Action ConnectionLost = delegate {};
        public bool Connected => tcpClient.Connected;

        private readonly TcpClient tcpClient;
        private readonly ILogger<RemotePoint> logger;
        private readonly BinaryReader reader;

        public string Ip => (tcpClient.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString()
            ?? "error while getting ip";
        public string Port => (tcpClient.Client.RemoteEndPoint as IPEndPoint).Port.ToString();

        public RemotePoint(TcpClient client, ILoggerFactory factory)
        {
            Pinger = new Pinger(SendMessage, factory.CreateLogger<Pinger>());
            reader = new BinaryReader(client.GetStream(), Encoding.UTF8, true);
            tcpClient = client;
            logger = factory.CreateLogger<RemotePoint>();
        }


        public async Task SendMessage(long packLength, Guid packId, MessageType type, Stream from)
        {
            var stream = tcpClient.GetStream();
            await sendingMessageSemaphore.WaitAsync();
            try
            {
                stream.Write(BitConverter.GetBytes(packLength));
                stream.Write(packId.ToByteArray());
                stream.WriteByte((byte)type);
                logger.LogDebug($"WRITE ID : {packId} Type : {type}");
                if (packLength - 17 == 0)
                    return;
                await from?.CopyPart(stream, (int)packLength - 17);
            }
            catch(Exception ex)
            {
                logger.LogWarning(ex, "Error while sending");
                ConnectionLost();
                throw;
            }
            finally
            {
                sendingMessageSemaphore.Release();
            }
        }

        public async Task<(long length, Guid packId, MessageType messageType, Stream data)> ReadMessage()
        {
            await readindMessageSemaphore.WaitAsync();
            try
            {
                long length = reader.ReadInt64();
                Guid packId = new Guid(reader.ReadBytes(16));
                MessageType type = (MessageType)reader.ReadByte();
                logger.LogDebug($"READ ID : {packId} Type : {type}");
                if (type == MessageType.PingResponse)
                {
                    Pinger.SetPing(packId);
                    return await ReadMessage();
                }
                return (length, packId, type, tcpClient.GetStream().Partial(length));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error while reading");
                ConnectionLost();
                throw;
            }
            finally
            {
                readindMessageSemaphore.Release();
            }
        }


        public void Dispose()
        {
            tcpClient.Dispose();
        }
    }

}
