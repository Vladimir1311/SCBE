using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CCF;
using System.IO;
using System.Net.Http;
using UEServersManager.Interfaces;
//using Microsoft.Owin;


namespace UDPServerTester.Controllers
{
    [Produces("application/json")]
    [Route("CCF/[action]")]
    public class CCFController : Controller
    {


        public IActionResult Send()
        {
            IRoomServerManager serverManager 
                = RemoteWorker<IRoomServerManager>.Create("http://localhost:51619");
            var result = serverManager.CreateServer();
            return Content(result.ToString());
        }


        [HttpPost]
        public IActionResult Recieve()
        {
            var form = HttpContext.Request.Form;
            string message = "";
            using (StreamReader reader = new StreamReader(HttpContext.Request.Body))
                message = reader.ReadToEnd();

            var service = new ServiceCode<IWorkInterface>(new Worker());
            var response = service.Handle(form);
            return Content(response);
        }

        private class Worker : IWorkInterface
        {
            public int StrLength(string str) => str?.Length ?? -1;
        }

    }
    public interface IWorkInterface
    {

        int StrLength(string str);
    }
}