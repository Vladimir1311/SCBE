using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IPResolver.Models;
using Microsoft.AspNetCore.SignalR;

namespace IPResolver.Pages.IP
{
    public class IndexHub : Hub
    {
        static HashSet<HubCallerContext> clients;
        readonly RemoteServicesManager servicesManager;

        public IndexHub(RemoteServicesManager servicesManager)
        {
            this.servicesManager = servicesManager;
            servicesManager.ServiceProviderAdded += SP =>
            {
                Clients.All.InvokeAsync("ServiceProviderAdded", 
                                        SP.InterfaceName,
                                        SP.providerPoint.Ip,
                                        SP.providerPoint.Port);
            };
        }

        public async override Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            clients.Add(Context);
        }
    }
}
