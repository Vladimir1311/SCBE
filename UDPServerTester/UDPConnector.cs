using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace UDPServerTester
{
    public class UdpConnector
    { 
        private UdpClient udpClient;
        private CancellationTokenSource cts;
        private ILogger<UdpConnector> _logger;

        //public event Action<FromClientPack> OnRecieveData;

        public UdpConnector(int port, ILogger<UdpConnector> logger)
        {
            udpClient = new UdpClient(port);
            _logger = logger;
        }


        public void Start()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            Task.Run(() => WorkCycle(token), token);
        }

        public void Stop()
        {
            cts.Cancel();
        }


        private async void WorkCycle(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var recieve = await udpClient.ReceiveAsync();
                _logger.LogInformation($"Recieved {recieve.Buffer.Length} bytes from {recieve.RemoteEndPoint.ToString()} adress family : {recieve.RemoteEndPoint.AddressFamily.ToString()}");
            }
            token.ThrowIfCancellationRequested();
        }

        public void SendPack(IPEndPoint endpoint, int port, byte[] data)
        {
            udpClient.SendAsync(data, data.Length, endpoint.Address.ToString(), port);
        }
    }
}
