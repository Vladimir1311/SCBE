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

namespace IPResolver.Services
{
    public class RemoteServicesManager
    {
        private TcpListener listener;

        private List<TCPService> servicesQueue = new List<TCPService>();
        private ConcurrentDictionary<string, TCPService> services = new ConcurrentDictionary<string, TCPService>();
        private List<TCPServiceUser> users = new List<TCPServiceUser>();
        private readonly ILogger<RemoteServicesManager> logger;

        public RemoteServicesManager(int port, ILogger<RemoteServicesManager> logger)
        {
            Port = port;
            while (true)
            {
                try
                {
                    listener = new TcpListener(IPAddress.Any, Port);
                    listener.Start();
                    break;
                }
                catch(Exception ex)
                {
                    logger.LogWarning(ex, $"Tryed port {Port}");
                    Port++;
                }
            }
            Task.Run(HandleClientsAccept);
            this.logger = logger;
        }

        internal int Port { get; private set; }

        internal bool HasService(string interfaceName) =>
            services.Any(P => P.Key == interfaceName);

        private async Task HandleClientsAccept()
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                logger.LogInformation($"accept tcp client {client.Client.RemoteEndPoint}");
                new Task(async () =>
                {
                    try
                    {
                        await HandleClient(client);
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
            var service = new TCPService
            {
                InterfaceName = interfaceName,
                Password = password
            };
            if (services.ContainsKey(service.InterfaceName))
                logger.LogInformation($"we have a service with name {interfaceName}");
            servicesQueue.Add(service);
        }

        internal void AddServiceUser(string interfaceName, string password)
        {
            logger.LogInformation($"adding user to {interfaceName} service");
            var user = new TCPServiceUser
            {
                InterfaceName = interfaceName,
                Password = password
            };
            users.Add(user);
        }

        internal List<TCPService> GetServices() =>
            services.Select(S => S.Value).ToList();

        private HashSet<Guid> pings = new HashSet<Guid>();

        internal void PingAll()
        {
            foreach (var service in services.Values)
            {
                Guid messageId = Guid.NewGuid();
                pings.Add(messageId);
                lock (service)
                {
                    var stream = service.Connection.GetStream();
                    stream.Write(BitConverter.GetBytes((long)17));
                    stream.Write(messageId.ToByteArray());
                    stream.WriteByte((byte)MessageType.PingRequest);
                }
            }
        }


        private async Task HandleClient(TcpClient client)
        {
            using (client)
            {
                string password;
                using (BinaryReader reader = new BinaryReader(client.GetStream(), Encoding.ASCII, true))
                {
                    password = reader.ReadString();
                }
                var service = servicesQueue.FirstOrDefault(S => S.Password == password);
                if (service != null)
                {
                    logger.LogInformation($"service for {service.InterfaceName} send correct password, replace old service to new");
                    services.AddOrUpdate(service.InterfaceName, service, (K, S) => service);
                    service.Connection = client;
                    service.ConnectionTime = DateTime.Now;
                    await HandleServiceLogic(service);
                    return;
                }


                var user = users.FirstOrDefault(U => U.Password == password);
                if (user == null)
                {
                    logger.LogWarning($"read incorrect data from tcpClient, closing connection");
                    return;
                }

                var targetService = services.FirstOrDefault(KV => KV.Value.InterfaceName == user.InterfaceName).Value;
                if (targetService == null)
                {
                    logger.LogWarning($"user sended correct, but target service {user.InterfaceName} was not found");
                    return;
                }
                logger.LogInformation($"user sended correct password, and service {targetService.InterfaceName} founded, bind user to service");
                user.Connection = client;
                user.ConnectionTime = DateTime.Now;
                await HandleClientLogic(user, targetService);
            }
        }

        private async Task HandleClientLogic(TCPServiceUser user, TCPService targetService)
        {
            try
            {
                targetService.Listeners.Add(user);
                using (var reader = new BinaryReader(user.Connection.GetStream(), Encoding.Unicode, true))
                {
                    while (true)
                    {
                        long packLength = reader.ReadInt64();
                        Guid packId = new Guid(reader.ReadBytes(16));
                        MessageType type = (MessageType)reader.ReadByte();
                        if (type == MessageType.PingResponse)
                        {
                            logger.LogDebug($"read ping response, wait for normal code");
                            user.LastPing = DateTime.Now;
                            continue;
                        }
                        logger.LogInformation($"read packet {packId} type {type} to service {targetService.InterfaceName}");
                        user.WaitedPacks.Add(packId);
                        var serviceStream = targetService.Connection.GetStream();
                        lock (targetService)
                        {
                            serviceStream.Write(BitConverter.GetBytes(packLength));
                            serviceStream.Write(packId.ToByteArray());
                            serviceStream.Write(new byte[] { (byte)type });
                            reader.BaseStream.CopyPart(serviceStream, (int)packLength - 17).Wait();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                targetService.Listeners.Remove(user);
                logger.LogWarning($"error with client {ex.Message}, deketing from listeners");
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
                        if (type == MessageType.PingResponse)
                        {
                            logger.LogInformation($"read packet {packId} type {type} from service {service.InterfaceName}");
                            service.LastPing = DateTime.Now;
                            continue;
                        }
                        logger.LogInformation($"read packet {packId} type {type} from service {service.InterfaceName}");
                        var targetUser = service.Listeners.FirstOrDefault(U => U.WaitedPacks.Contains(packId));
                        if (targetUser != null)
                        {
                            targetUser.WaitedPacks.Remove(packId);
                            var clientStream = targetUser.Connection.GetStream();
                            lock (targetUser)
                            {
                                clientStream.Write(BitConverter.GetBytes(packLength));
                                clientStream.Write(packId.ToByteArray());
                                clientStream.Write(new byte[] { (byte)type });
                                reader.BaseStream.CopyPart(clientStream, (int)packLength - 17).Wait();
                            }
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
                foreach (var client in service.Listeners)
                {
                    client.Connection.Dispose();
                    users.Remove(client);
                }
                services.TryRemove(service.InterfaceName, out _);
            }
        }
    }
}
