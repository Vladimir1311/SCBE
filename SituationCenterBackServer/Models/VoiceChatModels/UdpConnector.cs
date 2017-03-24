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

        public event Action<FromClientPack> OnRecieveData;

        public UdpConnector(int port)
        {
            udpClient = new UdpClient(port);
        }


        public void Start()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            Task.Run(() => WorkCycle(token), token);
        }



        public void SendPack(ToClientPack pack)
        {
            
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

        public void SendPack(IPEndPoint endpoint, int port, byte[] data)
        {
            udpClient.SendAsync(data, data.Length, endpoint.Address.ToString(), port);
        }
    }
}
