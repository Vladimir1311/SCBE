using IPResolver.Extensions;
using IPResolver.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IPResolver.Services
{
    public class RemoteServicesManager
    {
        private TcpListener listener;


        private ConcurrentDictionary<string, TCPService> services = new ConcurrentDictionary<string, TCPService>();
        private List<TCPServiceUser> users = new List<TCPServiceUser>();


        public RemoteServicesManager(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Task.Run(HandleClientsAccept);
        }


        private async Task HandleClientsAccept()
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                new Task(async () => await HandleClient(client)).Start();
            }
        }

        internal void AddService(string interfaceName, string password)
        {
            var service = new TCPService
            {
                InterfaceName = interfaceName,
                Password = password
            };
            services.AddOrUpdate(service.InterfaceName, service,
                (N, S) => service);
        }

        internal void AddServiceUser(string interfaceName, string password)
        {
            var user = new TCPServiceUser
            {
                InterfaceName = interfaceName,
                Password = password
            };
            users.Add(user);
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
                var service = services.FirstOrDefault(KV => KV.Value.Password == password).Value;
                if (service != null)
                {
                    service.Connection = client;
                    await HandleServiceLogic(client, service);
                    return;
                }


                var user = users.FirstOrDefault(U => U.Password == password);
                if (user == null)
                {
                    return;
                }

                var targetService = services.FirstOrDefault(KV => KV.Value.InterfaceName == user.InterfaceName).Value;
                if (targetService == null)
                {
                    return;
                }
                user.Connection = client;
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
                        user.WaitedPacks.Add(packId);
                        var ms = new MemoryStream();
                        ms.Write(BitConverter.GetBytes(packLength));
                        ms.Write(packId.ToByteArray());
                        var data = new byte[(int)packLength - 16];
                        await reader.BaseStream.ReadAsync(data, 0, data.Length);
                        ms.Write(data);
                        ms.Position = 0;
                        await ms.CopyToAsync(targetService.Connection.GetStream());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error with client {ex.Message}");
            }
        }

        private async Task HandleServiceLogic(TcpClient client, TCPService service)
        {
            try
            {
                using (var reader = new BinaryReader(client.GetStream(), Encoding.Unicode, true))
                {
                    while (true)
                    {
                        long packLength = reader.ReadInt64();
                        Guid packId = new Guid(reader.ReadBytes(16));
                        var ms = new MemoryStream();
                        ms.Write(BitConverter.GetBytes(packLength));
                        ms.Write(packId.ToByteArray());
                        var data = new byte[(int)packLength - 16];
                        await reader.BaseStream.ReadAsync(data, 0, data.Length);
                        ms.Write(data);
                        ms.Position = 0;
                        var targetUser = service.Listeners.FirstOrDefault(U => U.WaitedPacks.Contains(packId));
                        if (targetUser != null)
                        {
                            targetUser.WaitedPacks.Remove(packId);
                            await ms.CopyToAsync(targetUser.Connection.GetStream());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error with service {ex.Message}");
            }
        }
    }
}
