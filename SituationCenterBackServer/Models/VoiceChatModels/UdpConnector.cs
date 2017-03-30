using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class UdpConnector : IConnector
    {
        private UdpClient udpClient;
        private CancellationTokenSource cts;
        private ILogger<UdpConnector> _logger;

        public event Action<FromClientPack> OnRecieveData;

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



        public void SendPack(ToClientPack pack)
        {
            _logger.LogInformation($"Sending {pack.Data.Length} bytes from client {pack.ClientId} to {pack.IP.ToString()}");
            byte[] newData = pack.Data;
            
            if (pack.PackType == PackType.Voice)
            {
                newData = new byte[] { (byte)pack.PackType, pack.ClientId }.Concat(pack.Data.Select(B => (byte)(B < 50 ? 0 : B))).ToArray();
            }
            //udpClient.SendAsync(newData, newData.Length, pack.IP.Address.ToString(), 15000);
            udpClient.SendAsync(newData, newData.Length, pack.IP);
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
                _logger.LogInformation($"Received {recieve.Buffer.Length} bytes from {recieve.RemoteEndPoint.Address.ToString()}, Adress family : {recieve.RemoteEndPoint.AddressFamily}, port : {recieve.RemoteEndPoint.Port}");
                SendPack(new ToClientPack()
                {
                    ClientId = 1,
                    Data = recieve.Buffer,
                    IP = recieve.RemoteEndPoint,
                    PackType = PackType.Voice
                });
                OnRecieveData?.Invoke(new FromClientPack
                {
                    RoomId = recieve.Buffer[0],
                    ClientId = recieve.Buffer[1],
                    PackType = (PackType)recieve.Buffer[2],
                    VoiceRecord = recieve.Buffer.Skip(3).ToArray(),
                    IP = recieve.RemoteEndPoint
                });
            }
            token.ThrowIfCancellationRequested();
        }

        public void SendPack(IPEndPoint endpoint, byte[] data)
        {
            udpClient.SendAsync(data, data.Length, endpoint);
        }

        public void SendPack(IPEndPoint endpoint, int port, byte[] data)
        {
            udpClient.SendAsync(data, data.Length, endpoint.Address.ToString(), port);
        }
    }
}
