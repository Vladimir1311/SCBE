using IPResolver.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPResolver.Views.View
{
    public class IndexHub : Hub
    {
        private readonly RemoteServicesManager servicesManager;

        public IndexHub(RemoteServicesManager servicesManager)
        {
            this.servicesManager = servicesManager;
            servicesManager.ServiceClientAdded += N =>
            {
                Clients.All.InvokeAsync("newClient", N);
            };
        }


        public async Task Some(string message)
        {
            await Clients.All.InvokeAsync("newClient", message);
        }
    }
}
