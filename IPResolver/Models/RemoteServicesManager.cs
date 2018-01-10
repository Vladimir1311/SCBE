using CCF.Shared;
using IPResolver.Extensions;
using IPResolver.Models;
using IPResolver.Models.Points;
using IPResolver.Pages.IP;
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

        private List<(string password, string interfaceName)> clientsQueue = new List<(string password, string interfaceName)>();
        private List<(string password, string interfaceName)> serviceProvidersQueue = new List<(string password, string interfaceName)>();

        private List<RemotePoint> points = new List<RemotePoint>();
        private List<PointsLinker> linkedPairs = new List<PointsLinker>();
        
        private List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
        private readonly ILogger<RemoteServicesManager> logger;
        private readonly ILoggerFactory loggerFactory;

        public event Action<ServiceProvider> ServiceProviderAdded = delegate {};

        public ServiceProvider[] ServiceProviders => serviceProviders.ToArray();
        public PointsLinker[] PointsLinkers => linkedPairs.ToArray();
        public RemoteServicesManager(int port, ILoggerFactory loggerFactory, IndexHub hub)
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
            Task.Run(PingAll);
        }

        private async Task PingAll()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(15));
                try
                {
                    foreach (var provider in serviceProviders.ToArray())
                        try
                        {
                            await provider.Ping();
                        }
                        catch (Exception ex)
                        {
                            serviceProviders.Remove(provider);
                            logger.LogWarning($"Can't send ping to {provider.InterfaceName}", ex);
                        }
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Error while sending Ping", ex);
                }
            }

        }

        internal int Port { get; private set; }

        internal bool HasService(string interfaceName) =>
            serviceProviders.Any(P => P.InterfaceName == interfaceName);

        internal void AddToClientsQueue(string password, string interfaceName)
        {
            clientsQueue.Add((password, interfaceName));
        }


        internal void AddServiceProviderQueue(string interfaceName, string password)
        {
            serviceProvidersQueue.Add((password, interfaceName));
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


        private async Task HandleTcpConnection(TcpClient client)
        {

            string password;

            using (BinaryReader reader = new BinaryReader(client.GetStream(), Encoding.ASCII, true))
            {
                password = reader.ReadString();
            }
            var remotePoint = new RemotePoint(client, loggerFactory)
            {
                Password = password
            };
            if (serviceProviders.Any(P => P.WaitedClient(remotePoint)))
                return;

            if (serviceProvidersQueue.TryGet(P => P.password == password, out var providerPair))
            {

                var provider = new ServiceProvider(
                    remotePoint,
                    providerPair.interfaceName,
                    loggerFactory);
                serviceProviders.Add(provider);
                provider.providerPoint.ConnectionLost += () => {
                    serviceProviders.Remove(provider);
                };
                serviceProvidersQueue.Remove(providerPair);
                logger.LogDebug($"Registered serviceProvider with interface {providerPair.interfaceName}");
                ServiceProviderAdded(provider);
                await ProviderWork(provider);
                return;
            }

            if (clientsQueue.TryGet(C => C.password == password, out var clientPair))
            {
                if (!serviceProviders.TryGet(P => P.InterfaceName == clientPair.interfaceName,
                    out var serviceProvider))
                {
                    client.Dispose();
                    logger.LogWarning($"Service is not exist {clientPair.interfaceName}");
                    return;
                }
                var instance = await serviceProvider.GetServiceInstance(password);
                var link = new PointsLinker(remotePoint, instance, loggerFactory.CreateLogger<PointsLinker>());
                await link.StartConnection();
            }


        }

        private async Task ProviderWork(ServiceProvider provider)
        {
            try
            {
                await provider.HaveConnection();
            }
            catch
            {
                serviceProviders.Remove(provider);
                logger.LogInformation($"Service Provider disconnected");
            }
        }
    }
}
