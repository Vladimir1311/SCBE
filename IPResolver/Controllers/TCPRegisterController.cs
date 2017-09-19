using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IPResolver.Services;
using System.Text;

namespace IPResolver.Controllers
{
    [Produces("application/json")]
    [Route("ip/TCPRegister/[action]")]
    public class TCPRegisterController : Controller
    {
        
        private readonly RemoteServicesManager remoteServices;

        public TCPRegisterController(RemoteServicesManager remoteServices)
        {
            this.remoteServices = remoteServices;
        }

        public Response RegisterService(string interfaceName)
        {
            var password = CreatePassword();
            remoteServices.AddService(interfaceName, password);
            return new Response { Password = password, Port = 5476 };
        }

        public Response UseService(string interfaceName)
        {
            var password = CreatePassword();
            if(!remoteServices.HasService(interfaceName))
            {
                return new Response { Success = false };
            }
            remoteServices.AddServiceUser(interfaceName, password);
            return new Response { Password = password, Port = 5476 };
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