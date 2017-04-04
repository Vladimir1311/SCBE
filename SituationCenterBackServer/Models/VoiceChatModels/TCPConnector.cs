using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.VoiceChatModels
{
    public class TCPConnector : IConnector
    {

        public event Action<FromClientPack> OnRecieveData;


        private readonly TcpListener _listener;

        private CancellationTokenSource _cts;
        private readonly ILogger<TCPConnector> _logger;
        private readonly Dictionary<ApplicationUser, TcpClient> _connectionForUsers = new Dictionary<ApplicationUser, TcpClient>();
        private Func<byte, byte, ApplicationUser> _findUserFunc;


        public TCPConnector(ILogger<TCPConnector> logger, IOptions<UnrealAPIConfiguration> options)
        {
            _logger = logger;
            _listener = new TcpListener(IPAddress.Any, options.Value.Port);
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
            var bytestoSend = new[] {pack.User.InRoomId, (byte) pack.PackType}.Concat(pack.Data).ToArray();
            tcpClient.GetStream().Write(bytestoSend, 0, bytestoSend.Length);
        }
        public void SetBindToUser(Func<byte, byte, ApplicationUser> findUserFunc)
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
        private void HandleClient(TcpClient client, CancellationToken token)
        {
            _logger.LogInformation($"Client {client.Client.RemoteEndPoint}. Waiting for initial message.");
            var buffer = new byte[1024 * 8];
            var readed = client.GetStream().Read(buffer, 0, buffer.Length);
            if (readed < 3 || buffer[0] != (byte) PackType.Auth)
            {
                //TODO Оповещение пользователя о проблеме
                _logger.LogWarning("Client not sended auth message!!");
                client.Dispose();
                return;
            }
            var user = _findUserFunc(buffer[1], buffer[2]);
            if (user == null)
            {
                //TODO Оповещение пользователя о проблеме
                _logger.LogWarning($"Recieved pack with unreal parameters: userId: {buffer[0]} roomId: {buffer[1]}");
                client.Dispose();
                return;
            }
            _connectionForUsers[user] = client;
            do
            {
                readed = client.GetStream().ReadAsync(buffer, 0, buffer.Length, token).Result;
                _logger.LogDebug($"Recieved {readed} bytes from {client.Client.RemoteEndPoint}");
                OnRecieveData?.Invoke(new FromClientPack()
                {
                    User = user,
                    PackType = (PackType)buffer[0],
                    VoiceRecord = buffer.Skip(1).Take(readed).ToArray()
                });
            } while (readed != 0 && !token.IsCancellationRequested);
            client.Dispose();
            token.ThrowIfCancellationRequested();
        }

        
    } 
}
