﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SituationCenterBackServer.Models.VoiceChatModels.Connectors;
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
        //TODO Дописать класс для UDP подключения

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
            _logger.LogInformation($"Sending {pack.Data.Length} bytes to {pack.User.UserName}");
            if (_userEndPoints.TryGetValue(pack.User, out var endPoint))
            {
                udpClient.SendAsync(((byte)pack.PackType).Concat(pack.Data).ToArray(),
                    pack.Data.Length + 1,
                    endPoint);
                return;
            }
            _logger.LogWarning($"Try send info to User {pack.User.UserName}, but this user not connected via udp");
        }

        public void Stop() => cts.Cancel();

        private async void WorkCycle(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var recieve = await udpClient.ReceiveAsync();
                var buffer = recieve.Buffer;
                _logger.LogInformation($"Received {recieve.Buffer.Length} bytes from {recieve.RemoteEndPoint.Address.ToString()}, Adress family : {recieve.RemoteEndPoint.AddressFamily}, port : {recieve.RemoteEndPoint.Port}");
                var user = _userEndPoints.FirstOrDefault(Pair => Pair.Value.Equals(recieve.RemoteEndPoint)).Key;
                if (user == null)
                {
                    _logger.LogInformation($"Not authorized user with Address : {recieve.RemoteEndPoint}");
                    if (buffer[0] != (byte)PackType.Auth)
                    {
                        _logger.LogWarning($"User try send information without authorization : {recieve.RemoteEndPoint} Data : {recieve.Buffer.SumStrings()}");
                        continue;
                    }
                    var userToken = Encoding.UTF8.GetString(buffer, 1, buffer.Length - 1);
                    user = _findUserFunc(userToken);
                    if (user == null)
                    {
                        _logger.LogWarning($"User sended unreal token: {recieve.Buffer.SumStrings().Replace(", ", "")} {recieve.RemoteEndPoint}");
                        continue;
                    }
                    _userEndPoints[user] = recieve.RemoteEndPoint;
                    OnUserConnected?.Invoke(user);
                    continue;
                }

                OnRecieveData?.Invoke(new FromClientPack
                {
                    User = user,
                    PackType = (PackType)recieve.Buffer[2],
                    Data = buffer.Skip(1).Take(buffer.Length - 1).ToArray()
                });
            }
            token.ThrowIfCancellationRequested();
        }

        public void SetBindToUser(Func<string, ApplicationUser> findUserFunc)
        {
            _findUserFunc = findUserFunc;
        }
    }
}
