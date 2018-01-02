using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CCF.Extensions;
using Microsoft.Extensions.Logging;
using CCF.Shared;
using System.Threading;
using CCF.Messages;

namespace CCF.Transport
{

    class TCPTransporter : ITransporter
    {
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public event Func<InvokeMessage, Task> OnReceiveMessge;
        public event Func<InvokeResult, Task> OnReceiveResult;
        public event Func<string, Task> OnNeedNewService;
        public event Action OnConnectionLost;

        private object locker = new object();
        private TcpClient tcpClient;
        private readonly ILogger<TCPTransporter> logger;

        public TCPTransporter(string host, int port, string password, ILogger<TCPTransporter> logger)
        {
            tcpClient = new TcpClient();
            tcpClient.ConnectAsync(host, port).Wait();
            using (var writer = new BinaryWriter(tcpClient.GetStream(), Encoding.ASCII, true))
                writer.Write(password);
            new Task(
                async () =>
                {
                    try
                    {
                        await ReadStream();
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning($"Connection was aborted, exception: {ex.Message}");
                        OnConnectionLost?.Invoke();
                    }
                }).Start();
            this.logger = logger;
        }

        public async Task SendMessage(InvokeMessage message)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                logger.LogDebug($"sending message {message.Id}");
                var streamToOut = EncodeMessage(message);
                streamToOut.Position = 0;
                await streamToOut.CopyToAsync(tcpClient.GetStream());
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task SendResult(InvokeResult result)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                logger.LogDebug($"sending result {result.Id}");
                var streamToOut = EncodeResult(result);
                streamToOut.Position = 0;
                await streamToOut.CopyToAsync(tcpClient.GetStream());
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task SetupNewService(Guid packId, int serviceId)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                using (var writer = new BinaryWriter(tcpClient.GetStream(), Encoding.UTF8, true))
                {
                    writer.Write((long)(16 + 1));
                    writer.Write(packId.ToByteArray());
                    writer.Write((byte)MessageType.CreateInstanceResponse);
                    writer.Write(serviceId);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task SendPingResponse(Guid id)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                using (var writer = new BinaryWriter(tcpClient.GetStream(), Encoding.UTF8, true))
                {
                    writer.Write((long)(16 + 1));
                    writer.Write(id.ToByteArray());
                    writer.Write((byte)MessageType.PingResponse);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task ReadStream()
        {
            var forSize = new byte[8];
            var forGuid = new byte[16];
            var forType = new byte[1];
            var clientStream = tcpClient.GetStream();
            while (true)
            {
                await clientStream.ReadAsync(forSize, 0, 8);
                var size = BitConverter.ToInt64(forSize, 0);

                await clientStream.ReadAsync(forGuid, 0, 16);
                var id = new Guid(forGuid);

                await clientStream.ReadAsync(forType, 0, 1);
                var type = (MessageType)forType[0];
                logger.LogDebug($"read packetId {id} type is {type}");

                var contentStream = new MemoryStream();
                //TODO обрезание long в int!!!!
                await clientStream.CopyPart(contentStream, (int)size - 17);

                contentStream.Position = 0;
                try
                {
                    switch (type)
                    {
                        case MessageType.Message:
                            InvokeMessage message = DecodeMessage(contentStream, id);
                            await OnReceiveMessge?.Invoke(message);
                            logger.LogDebug($"invoked message handler for {id}, go to new iteration");
                            break;
                        case MessageType.Result:
                            InvokeResult result = DecodeResult(contentStream, id);
                            await OnReceiveResult?.Invoke(result);
                            logger.LogDebug($"invoked result handler for {id}, go to new iteration");
                            break;
                        case MessageType.PingRequest:
                            logger.LogDebug($"Request for ping");
                            await SendPingResponse(id);
                            logger.LogDebug($"Sended ping response");
                            break;
                        case MessageType.CreateInstanceRequest:
                            logger.LogDebug("need new service instance");
                            var password = await ReadPassword(contentStream, (int)size - 17);
                            await OnNeedNewService?.Invoke(password);
                            break;
                        default:
                            logger.LogWarning($"incorrect message type {type}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EXCEPTION {ex.Message}");
                }
            }
        }

        private async Task<string> ReadPassword(Stream stream, int length)
        {
            var bytes = new byte[length];
            await stream.ReadAsync(bytes, 0, length);
            //ASCII FOR PASSWORD!!!
            return Encoding.ASCII.GetString(bytes);
        }

        private InvokeMessage DecodeMessage(MemoryStream contentStream, Guid id)
        {
            InvokeMessage message = new InvokeMessage();
            using (var reader = new BinaryReader(contentStream, Encoding.UTF8))
            {
                message.Id = id;
                message.SubObjectId = reader.ReadInt32();
                message.MethodName = reader.ReadString();
                message.Args = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(reader.ReadString());
                message.Streams = new Dictionary<string, Stream>();
                while (contentStream.Position != contentStream.Length)
                {
                    var streamName = reader.ReadString();
                    var size = (int)reader.ReadInt64();
                    var innerStream = new MemoryStream();
                    contentStream.CopyPart(innerStream, size).Wait();
                    innerStream.Position = 0;
                    message.Streams[streamName] = innerStream;
                }
            }
            return message;
        }

        private Stream EncodeMessage(InvokeMessage message)
        {
            var outStream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(outStream, Encoding.UTF8, true))
            {
                writer.Write(long.MaxValue);
                writer.Write(message.Id.ToByteArray());
                writer.Write((byte)MessageType.Message);
                //---
                writer.Write(message.SubObjectId);
                //---
                writer.Write(message.MethodName);
                //---
                writer.Write(JsonConvert.SerializeObject(message.Args));
                //---
                foreach (var keyStream in message.Streams)
                {
                    writer.Write(keyStream.Key);
                    WriteStream(writer, keyStream.Value);
                }
                outStream.Position = 0;
                writer.Write(outStream.Length - sizeof(long));
            }
            return outStream;
        }
        private InvokeResult DecodeResult(MemoryStream contentStream, Guid id)
        {
            InvokeResult result = new InvokeResult();
            using (var reader = new BinaryReader(contentStream, Encoding.UTF8))
            {
                result.Id = id;
                result.SubObjectId = reader.ReadInt32();
                result.IsPrimitive = reader.ReadBoolean();
                result.Value = JToken.Parse(reader.ReadString());
                if (contentStream.Position != contentStream.Length)
                {
                    var size = (int)reader.ReadInt64();
                    var innerStream = new MemoryStream();
                    contentStream.CopyPart(innerStream, size).Wait();
                    result.StreamValue = innerStream;
                    result.StreamValue.Position = 0;
                }
            }
            return result;
        }

        private Stream EncodeResult(InvokeResult result)
        {
            var outStream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(outStream, Encoding.UTF8, true))
            {
                writer.Write(long.MaxValue);
                writer.Write(result.Id.ToByteArray());
                writer.Write((byte)MessageType.Result);
                writer.Write(result.SubObjectId);
                writer.Write(result.IsPrimitive);
                writer.Write(result.Value?.ToString(Formatting.Indented) ?? JValue.CreateNull().ToString(Formatting.Indented));
                if (result.StreamValue != null)
                    WriteStream(writer, result.StreamValue);
                outStream.Position = 0;
                writer.Write(outStream.Length - sizeof(long));
            }
            return outStream;
        }

        private void WriteStream(BinaryWriter writer, Stream stream)
        {
            stream.Position = 0;
            writer.Write(stream.Length);
            stream.CopyTo(writer.BaseStream);
        }

        public void Dispose()
        {
            tcpClient.Dispose();
        }
    }
}
