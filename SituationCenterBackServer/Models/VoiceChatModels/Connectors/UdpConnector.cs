using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels.Connectors
{
    public class UdpConnector : IConnector
    {
        private UdpClient udpClient;
        private CancellationTokenSource cts;
        private ILogger<UdpConnector> _logger;

        private Dictionary<ApplicationUser, IPEndPoint> _userEndPoints = new Dictionary<ApplicationUser, IPEndPoint>();

        public event Action<FromClientPack> OnRecieveData;

        public event Action<ApplicationUser> OnUserConnected;

        private Func<string, ApplicationUser> _findUserFunc;

        public UdpConnector(ILogger<UdpConnector> logger, IOptions<UnrealAPIConfiguration> config)
        {
            udpClient = new UdpClient(config.Value.UdpPort);
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
            if (!_userEndPoints.TryGetValue(pack.Receiver, out var endPoint))
            {
                _logger.LogWarning($"Try send info to User {pack.Receiver.UserName}, but this user not connected via UDP");
                return;
            }
            var packedData = ((byte)pack.PackType).Concat(pack.Data).ToArray();
            //_logger.LogInformation($"Sending {packedData.Length} bytes to {pack.Receiver.UserName}, first elements: {packedData[0]} {packedData[1]} {packedData[2]}");
            udpClient.SendAsync(packedData,
                packedData.Length,
                endPoint);
        }

        public void Stop() => cts.Cancel();

        private async void WorkCycle(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                UdpReceiveResult recieve;
                try
                {
                    recieve = await udpClient.ReceiveAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                    continue;
                }
                var buffer = recieve.Buffer;
                _logger.LogInformation($"Received {recieve.Buffer.Length} bytes from {recieve.RemoteEndPoint.Address.ToString()}, Adress family : {recieve.RemoteEndPoint.AddressFamily}, port : {recieve.RemoteEndPoint.Port}");
                if (IsAuth(buffer))
                {
                    Auth(recieve.Buffer, recieve.RemoteEndPoint);
                    continue;
                }
                var user = GetSenderFromEndPoint(recieve.RemoteEndPoint);
                if (user == null)
                {
                    _logger.LogInformation($"Not authorized user with Address : {recieve.RemoteEndPoint}");
                    continue;
                }

                OnRecieveData?.Invoke(new FromClientPack
                {
                    User = user,
                    PackType = (PackType)recieve.Buffer[0],
                    Data = buffer.Skip(1).Take(buffer.Length - 1).ToArray()
                });
            }
            token.ThrowIfCancellationRequested();
        }

        public void SetBindToUser(Func<string, ApplicationUser> findUserFunc)
        {
            _findUserFunc = findUserFunc;
        }

        private bool IsAuth(byte[] buffer) => buffer[0] == (byte)PackType.Auth;

        private string ReadToken(byte[] buffer) => Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1);

        private ApplicationUser GetSenderFromEndPoint(IPEndPoint endpoint) =>
            _userEndPoints.FirstOrDefault(Pair => Pair.Value.Equals(endpoint)).Key;

        private void Auth(byte[] buffer, IPEndPoint endpoint)
        {
            var userToken = ReadToken(buffer);
            var user = _findUserFunc(userToken);
            if (user == null)
                _logger.LogWarning($"User sended unreal token: {buffer.SumStrings().Replace(", ", "")} {endpoint}");
            _userEndPoints[user] = endpoint;
            OnUserConnected?.Invoke(user);
        }
    }
}