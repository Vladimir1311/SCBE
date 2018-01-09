using System;
using IPResolver.Models;
using Microsoft.AspNetCore.SignalR;

namespace IPResolver.Pages.IP
{
    public class IndexHub : Hub
    {
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


    }
}
