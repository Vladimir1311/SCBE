using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPResolver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPResolver.Controllers
{
    [Produces("application/json")]
    [Route("ip/TCPRegister/[action]")]
    public class TCPRegisterController : Controller
    {

        private readonly RemoteServicesManager servicesManager;

        public TCPRegisterController(RemoteServicesManager remoteServices)
        {
            servicesManager = remoteServices;
        }

        public Response RegisterServiceProvider(string interfaceName)
        {
            var password = CreatePassword();
            servicesManager.AddServiceProviderQueue(interfaceName, password);
            return new Response { Password = password, Port = servicesManager.Port };
        }

        public Response ConnectoToService(string interfaceName)
        {
            var password = CreatePassword();
            if (!servicesManager.HasService(interfaceName))
                return new Response { Success = false };
            servicesManager.AddToClientsQueue(password, interfaceName);
            return new Response { Password = password, Port = servicesManager.Port};
        }

        private string CreatePassword()
        {
            var bytes = new byte[20];
            new Random().NextBytes(bytes);
            return Encoding.ASCII.GetString(bytes);
        }
    }
    public class Response
    {
        public bool Success { get; set; } = true;
        public string Password { get; set; }
        public int Port { get; set; }
    }
}