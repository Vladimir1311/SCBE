using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class TCPConnector
    {
        private TcpListener listener;

        private CancellationTokenSource cts;
        private ILogger<TCPConnector> _logger;

        public TCPConnector(ILogger<TCPConnector> logger)
        {
            _logger = logger;
            //IPAddress address = IPAddress.Parse("13.84.55.187"); VM
            IPAddress address = IPAddress.Parse("127.0.0.1"); 
            listener = new TcpListener(address, 11000);
            listener.Start();
            
            cts = new CancellationTokenSource();
            var token = cts.Token;
            Task.Factory.StartNew(() => Listen(token), token);
        }

        private void Listen(CancellationToken token)
        {
            while (true)
            {
                _logger.LogInformation("Waiting for client...");
                var client= listener.AcceptTcpClientAsync().Result;

                if (client != null)
                {
                    Task.Factory.StartNew(() => HandleClient(client));
                }
            }
        }

        void HandleClient(TcpClient client)
        {
            _logger.LogInformation($"Client {client.Client.RemoteEndPoint.ToString()}. Waiting for data.");
            byte[] buffer = new byte[1024];
            var readed = client.GetStream().Read(buffer, 0, buffer.Length);
            _logger.LogInformation($"Readed {readed} bytes");
            client.GetStream().Write(buffer, 0, readed);
            _logger.LogInformation($"Sended test {readed} bytes");
            _logger.LogInformation("Closing connection.");
            client.GetStream().Dispose();
        }
    } 
}
