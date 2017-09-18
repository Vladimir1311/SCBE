using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IPResolver.Services;
using IPResolver.Models;

namespace IPResolver.Controllers
{
    [Route("ip/view/[action]")]
    public class ViewController : Controller
    {
        private readonly RemoteServicesManager manager;

        public ViewController(RemoteServicesManager manager)
        {
            this.manager = manager;
        }
        public IActionResult Index()
        {
            ViewModel data = new ViewModel
            {
                Services = manager.GetServices()
            };
            
            return View(data);
        }

        public class ViewModel
        {
            public List<TCPService> Services { get; internal set; }
        }
    }
}