﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SituationCenterBackServer.Extensions;
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
    public class TCPConnector : IStableConnector
    {

        public event Action<FromClientPack> OnRecieveData;
        public event Action<ApplicationUser> OnUserConnected;


        private readonly TcpListener _listener;

        private CancellationTokenSource _cts;
        private readonly ILogger<TCPConnector> _logger;
        private readonly Dictionary<ApplicationUser, TcpClient> _connectionForUsers = new Dictionary<ApplicationUser, TcpClient>();
        private Func<string, ApplicationUser> _findUserFunc;


        public TCPConnector(ILogger<TCPConnector> logger, IOptions<UnrealAPIConfiguration> options)
        {
            _logger = logger;
            _listener = new TcpListener(IPAddress.Any, options.Value.TcpPort);
        }

        public void Start()
        {
            if (_cts != null)
            {
                _logger.LogWarning("Try to Start started TCPConnector");
                return;
            }
            if (_findUserFunc == null)
                throw new Exception("You must define function for find user, use SetBindToUser method for this");
            _listener.Start();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            Task.Factory.StartNew(() => Listen(token), token);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        public void SendPack(ToClientPack pack)
        {
            if (!_connectionForUsers.ContainsKey(pack.User))
            {
                _logger.LogWarning("User " + pack.User.Email + " not found in TCP connections");
                return;
            }
            var tcpClient = _connectionForUsers[pack.User];
            if (!tcpClient.Connected)
            {
                _logger.LogWarning($"User {pack.User.Email} not connected ovet TCP");
                return;
            }
            var bytestoSend = CreateHeader(pack.Data.Length + 2).Concat(new[] { (byte)pack.PackType, pack.User.InRoomId,}.Concat(pack.Data)).ToArray();
            tcpClient.GetStream().WriteAsync(bytestoSend, 0, bytestoSend.Length).Wait();
        }
        public void SetBindToUser(Func<string, ApplicationUser> findUserFunc)
        {
            _findUserFunc = findUserFunc;
        }
        private void Listen(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for client...");
                var client= _listener.AcceptTcpClientAsync().Result;
                if (client != null)
                {
                    Task.Factory.StartNew(() => HandleClient(client, token), token);
                }
                token.ThrowIfCancellationRequested();
            }
        }
        private async void HandleClient(TcpClient client, CancellationToken token)
        {
            _logger.LogInformation($"Client {client.Client.RemoteEndPoint}. Waiting for initial message.");
            var buffer = new byte[1024 * 8];
            var readed = await client.ReadPacketAsync(buffer, 0, buffer.Length, token);
            if (buffer[0] != (byte) PackType.Auth)
            {
                //TODO Оповещение пользователя о проблеме
                _logger.LogWarning("Client not sended auth message!!");
                client.Dispose();
                return;
            }
            var userId = Encoding.UTF8.GetString(buffer, 1, readed - 1);
            var user = _findUserFunc(userId);
            if (user == null)
            {
                //TODO Оповещение пользователя о проблеме
                _logger.LogWarning($"Recieved pack with unreal parameters: userId: {userId}");
                client.Dispose();
                return;
            }
            _logger.LogInformation($"Connected user {user.UserName}");
            _connectionForUsers[user] = client;
            OnUserConnected(user);
            do
            {
                readed = await client.ReadPacketAsync(buffer, 0, buffer.Length, token);
                _logger.LogDebug($"Recieved {readed} bytes from {client.Client.RemoteEndPoint}, {user.UserName}");
                OnRecieveData?.Invoke(new FromClientPack()
                {
                    User = user,
                    PackType = (PackType)buffer[0],
                    Data = buffer.Skip(1).Take(readed-1).ToArray()
                });
            } while (readed != 0 && !token.IsCancellationRequested);
            client.Dispose();
            _logger.LogInformation($"User {user.Email} disconnected ");
            token.ThrowIfCancellationRequested();
        }

        private byte[] CreateHeader(int value)
        {
            return new byte[] { (byte)(value >> 8), (byte)(value)};
        }
    } 
}