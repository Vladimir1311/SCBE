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
            var id = Guid.Parse("3b307812-416e-4abf-a889-2eea295cec18");
        }
        public IActionResult Index()
        {
            ViewModel data = new ViewModel
            {
                Services = manager.GetServices(),
                Users = manager.GetUsers()
            };
            
            return View(data);
        }

        public class ViewModel
        {
            public List<TCPService> Services { get; internal set; }
            public List<TCPServiceUser> Users { get; internal set; }
        }
    }
}