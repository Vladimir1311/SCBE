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

namespace CCF.Transport
{
    
    class TCPTransporter : ITransporter
    {
        private enum MessageType : byte
        {
            Message, Result
        }
        public event Action<InvokeMessage> OnReceiveMessge;
        public event Action<InvokeResult> OnReceiveResult;

        private TcpClient tcpClient;
        private readonly ILogger<TCPTransporter> logger;

        public TCPTransporter(string host, int port, string password, ILogger<TCPTransporter> logger)
        {
            tcpClient = new TcpClient();
            tcpClient.ConnectAsync(host, port).Wait();
            using (var writer = new BinaryWriter(tcpClient.GetStream(), Encoding.ASCII, true))
                writer.Write(password);
            new Task(async () => await ReadStream()).Start();
            this.logger = logger;
        }

        public void SendMessage(InvokeMessage message)
        {
            logger.LogDebug($"sending message {message.Id}");
            var streamToOut = EncodeMessage(message);
            streamToOut.Position = 0;
            streamToOut.CopyTo(tcpClient.GetStream());
        }

        public void SendResult(InvokeResult result)
        {
            logger.LogDebug($"sending result {result.Id}");
            var streamToOut = EncodeResult(result);
            streamToOut.Position = 0;
            streamToOut.CopyTo(tcpClient.GetStream());
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
                switch (type)
                {
                    case MessageType.Message:
                        InvokeMessage message = DecodeMessage(contentStream, id);
                        OnReceiveMessge?.Invoke(message);
                        logger.LogDebug($"invoked message handler for {id}, go to new iteration");
                        break;
                    case MessageType.Result:
                        InvokeResult result = DecodeResult(contentStream, id);
                        OnReceiveResult?.Invoke(result);
                        logger.LogDebug($"invoked result handler for {id}, go to new iteration");
                        break;
                    default:
                        break;
                }
            }
        }


        private InvokeMessage DecodeMessage(MemoryStream contentStream, Guid id)
        {
            InvokeMessage message = new InvokeMessage();
            using (var reader = new BinaryReader(contentStream, Encoding.Unicode))
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
            using (BinaryWriter writer = new BinaryWriter(outStream, Encoding.Unicode, true))
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
            using (var reader = new BinaryReader(contentStream, Encoding.Unicode))
            {
                result.Id = id;
                result.SubObjectId = reader.ReadInt32();
                result.IsPrimitive = reader.ReadBoolean();
                result.Value = JToken.Parse(reader.ReadString());
                if(contentStream.Position != contentStream.Length)
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
            using (BinaryWriter writer = new BinaryWriter(outStream, Encoding.Unicode, true))
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
    }
}
