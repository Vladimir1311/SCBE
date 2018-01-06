using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPResolver.Models;
using IPResolver.Models.Points;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IPResolver.Pages.IP.Index
{
    public class IndexModel : PageModel
    {
        public IEnumerable<ServiceProvider> Providers { get; private set; }

        readonly RemoteServicesManager manager;

        public IndexModel(RemoteServicesManager manager)
        {
            this.manager = manager;
        }
        public void OnGet()
        {
            Providers = manager.ServiceProviders;
        }
    }
}
