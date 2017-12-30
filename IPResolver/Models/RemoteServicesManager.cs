using CCF.Shared;
using IPResolver.Extensions;
using IPResolver.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPResolver.Models
{
    public class RemoteServicesManager
    {
        private TcpListener listener;

        private List<RemotePoint> points = new List<RemotePoint>();
        private ConcurrentDictionary<Guid, RemotePoint> waitedMessages = new ConcurrentDictionary<Guid, RemotePoint>();

        private readonly ILogger<RemoteServicesManager> logger;
        private readonly ILoggerFactory loggerFactory;

        public RemoteServicesManager(int port, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<RemoteServicesManager>();
            Port = port;
            this.loggerFactory = loggerFactory;
            while (true)
            {
                try
                {
                    listener = new TcpListener(IPAddress.Any, Port);
                    listener.Start();
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"Tryed port {Port}");
                    Port++;
                }
            }
            Task.Run(HandleTcpConnectionAccept);
            Task.Run(PingSending);
        }

        internal int Port { get; private set; }

        internal bool HasService(string interfaceName) =>
            points.Any(P => P.SelfInterfaceName == interfaceName);


        private async Task PingSending()
        {
            var maxTimeOut = TimeSpan.FromSeconds(10);
            while (true)
            {
                await PingAll();
                var now = DateTime.Now;
                await Task.Delay(TimeSpan.FromMinutes(1));
                var toRemove = new List<RemotePoint>();
                foreach (var endPoint in points)
                {
                    if (endPoint.Pinger.LastPing - now > maxTimeOut)
                    {
                        endPoint.Dispose();
                        toRemove.Add(endPoint);
                    }
                }
                toRemove.ForEach(P => points.Remove(P));
            }
        }

        private async Task PingAll()
        {
            foreach (var service in points)
            {
                try
                {
                    await service.Pinger.SendPing();
                }
                catch (Exception ex)
                {
                    logger.LogDebug($"error with sendind ping to EndPoint {service.IPAddress} exception {ex.Message}");
                }
            }
        }

        private async Task HandleTcpConnectionAccept()
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                logger.LogInformation($"accept tcp client {client.Client.RemoteEndPoint}");
                new Task(async () =>
                {
                    try
                    {
                        await HandleTcpConnection(client);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning($"error with handling client {ex.Message}");
                    }
                }).Start();
            }
        }

        internal void AddService(string interfaceName, string password)
        {
            logger.LogInformation($"register service with name {interfaceName}");
            var service = new RemotePoint(loggerFactory)
            {
                SelfInterfaceName = interfaceName,
                Password = password
            };
            points.Add(service);
        }

        internal async Task<int> AddServiceUser(string interfaceName, string password)
        {
            logger.LogInformation($"adding user to {interfaceName} service");
            var user = new RemotePoint(loggerFactory)
            {
                SelfInterfaceName = interfaceName,
                Password = password,
            };
            var service = points.FirstOrDefault(P => P.SelfInterfaceName == interfaceName);
            if (service == null)
                return -1;
            points.Add(user);
            return await service.CreateInstanse();
        }


        private async Task HandleTcpConnection(TcpClient client)
        {
            using (client)
            {
                string password;
                using (BinaryReader reader = new BinaryReader(client.GetStream(), Encoding.ASCII, true))
                {
                    password = reader.ReadString();
                }
                var point = points.FirstOrDefault(S => S.Password == password);
                if (point == null)
                {
                    logger.LogInformation("Incorrect password");
                    return;
                }
                if (point.SelfInterfaceName != null)
                {
                    logger.LogInformation($"point with {point.SelfInterfaceName} interface correct password, adding to list");
                    points.Add(point);
                    point.InitConnection(client);
                    point.ConnectionTime = DateTime.Now;
                    await HandleServiceLogic(point);
                    return;
                }


                if (point.WantedInterfaceName == null)
                {
                    logger.LogWarning($"read incorrect data from tcpClient, closing connection");
                    return;
                }

                var targetService = points.LastOrDefault(P => P.SelfInterfaceName == point.WantedInterfaceName);
                if (targetService == null)
                {
                    logger.LogWarning($"user sended correct, but target service {point.WantedInterfaceName} was not found");
                    return;
                }
                logger.LogInformation($"user sended correct password, and service {point.WantedInterfaceName} founded, bind user to service");
                point.InitConnection(client);
                point.ConnectionTime = DateTime.Now;
                await HandleClientLogic(user, targetService);
            }
        }

        private async Task HandleClientLogic(RemotePoint user, RemotePoint targetService)
        {
            try
            {
                //ServiceClientAdded(targetService.InterfaceName);
                targetService.Listeners.Add(user);
                using (var reader = new BinaryReader(user.Connection.GetStream(), Encoding.Unicode, true))
                {
                    while (true)
                    {
                        var (packLength, packId, type) = user.ReadHeader();
                        if (type == MessageType.PingResponse)
                        {
                            logger.LogDebug($"read ping response, wait for normal code");
                            user.Pinger.SetPing(packId);
                            continue;
                        }
                        logger.LogTrace($"read packet {packId} type {type} to service {targetService.SelfInterfaceName}");
                        user.WaitedPacks.Add(packId);

                        await targetService.SendMessage((int)packLength, packId, type, reader.BaseStream);

                    }
                }
            }
            catch (Exception ex)
            {
                targetService.Listeners.Remove(user);
                logger.LogWarning($"error with client for service {targetService.InterfaceName} {ex.Message}, deleting from listeners");
            }
        }

        private async Task HandleServiceLogic(TCPService service)
        {
            try
            {
                using (var reader = new BinaryReader(service.Connection.GetStream(), Encoding.Unicode, true))
                {
                    while (true)
                    {
                        long packLength = reader.ReadInt64();
                        Guid packId = new Guid(reader.ReadBytes(16));
                        MessageType type = (MessageType)reader.ReadByte();
                        switch (type)
                        {
                            case MessageType.PingResponse:
                                logger.LogInformation($"read ping response {packId} from service {service.InterfaceName}");
                                service.SetPing(packId);
                                continue;
                            case MessageType.ServiceCreateResponse:
                                logger.LogDebug("get new service instanse id");
                                var readedInt = reader.ReadInt32();
                                service.SetInstaceCreating(packId, readedInt);
                                continue;
                            default: break;
                        }
                        logger.LogInformation($"read packet {packId} type {type} from service {service.InterfaceName}");
                        var targetUser = service.Listeners.FirstOrDefault(U => U.WaitedPacks.Contains(packId));
                        if (targetUser != null)
                        {
                            targetUser.WaitedPacks.Remove(packId);
                            await targetUser.SendMessage((int)packLength, packId, type, reader.BaseStream);
                        }
                        else
                        {
                            logger.LogWarning($"service send pack {packId}, but not found client for this packet");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"connection to service {service.InterfaceName} aborted.");
                logger.LogWarning($"error with service {ex.Message}");
                services.TryRemove(service.InterfaceName, out _);
                foreach (var client in service.Listeners)
                {
                    client.Connection.Dispose();
                    users.Remove(client);
                }
            }
        }
    }
}
