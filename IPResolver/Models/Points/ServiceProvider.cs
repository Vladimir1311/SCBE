using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace IPResolver.Models.Points
{
    public class ServiceProvider
    {
        private RemotePoint providerPoint;
        private ILogger<ServiceProvider> logger;

        private HashSet<WaitClientPack> waitPacks = new HashSet<WaitClientPack>();
        public string InterfaceName { get; }

        public ServiceProvider(RemotePoint remotePoint, string interfaceName, ILoggerFactory factory)
        {
            providerPoint = remotePoint;
            InterfaceName = interfaceName;
            logger = factory.CreateLogger<ServiceProvider>();
        }

        public async Task HaveConnection()
        {
            await Task.CompletedTask;
            while (true)
            {
                var data = providerPoint.ReadMessage();
                logger.LogError("WTF?! Service provider send a message!");
            }
        }

        public async Task<RemotePoint> GetServiceInstance(string instancePassword)
        {
            var passwordBytes = Encoding.ASCII.GetBytes(instancePassword);
            var waiter = new WaitClientPack
            {
                password = instancePassword,
                semaphore = new SemaphoreSlim(0, 1)
            };
            waitPacks.Add(waiter);
            await providerPoint.SendMessage(
                passwordBytes.Length + 17,
                Guid.Empty, 
                CCF.Shared.MessageType.CreateInstanceRequest, 
                new MemoryStream(passwordBytes));
            await waiter.semaphore.WaitAsync();
            return waiter.point;
        }


        public bool WaitedClient(RemotePoint point)
        {
            var targetWaitPack = waitPacks.FirstOrDefault(P => P.password == point.Password);
            if (targetWaitPack == null) return false;
            targetWaitPack.point = point;
            waitPacks.Remove(targetWaitPack);
            targetWaitPack.semaphore.Release();
            return true;
        }

        public async Task Ping() => await providerPoint.Pinger.SendPing();


        private class WaitClientPack
        {
            public string password;
            public RemotePoint point;
            public SemaphoreSlim semaphore;
        }
    }
}
