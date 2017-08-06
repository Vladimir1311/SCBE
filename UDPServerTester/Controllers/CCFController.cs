using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CCF;
using System.IO;
using System.Net.Http;


namespace UDPServerTester.Controllers
{
    [Produces("application/json")]
    [Route("CCF/[action]")]
    public class CCFController : Controller
    {
        private static IRoomServerManager rsm;
        private static ServiceCode service = ServiceCode.Create(new rsm());


        public CCFController()
        {
            rsm
                = RemoteWorker.Create<IRoomServerManager>("http://localhost:55279");
        }
        public IActionResult Send()
        {
            var result = rsm.CreateServer();
            return Content(result.ToString());
        }

        public IActionResult Kill(int port)
        {
            var server = rsm.GetRoomServer(port);
            var success = server.Kill();
            return Content(success.ToString());
        }

        public IActionResult List()
        {
            var list = rsm.GetAllRoomServer();
            return Content(list.Count().ToString());
        }
        public IActionResult Recieve()
        {
            var res = service.Handle(Request.Form);
            return Content(res);
        }

    }

    public interface IRoomServerManager
    {
        int CreateServer();
        IEnumerable<IRoomServer> GetAllRoomServer();
        IRoomServer GetRoomServer(int Port);
    }
    public interface IRoomServer
    {
        int Port { get; }

        bool Kill();
        void ResiveMessage(string massage);
    }

    class rsm : IRoomServerManager
    {
        private List<Server> servers = new List<Server>();
        private static int port = 5000;
        public int CreateServer()
        {
            servers.Add(new Server(port++));
            return servers.Last().Port;
        }

        public IEnumerable<IRoomServer> GetAllRoomServer()
        {
            return servers;
        }

        public IRoomServer GetRoomServer(int Port)
        {
            return servers.FirstOrDefault(S => S.Port == Port);
        }
    }

    class Server : IRoomServer
    {

        public int Port { get; private set; }

        public Server(int port)
        {
            Port = port;
        }

        public bool Kill()
        {
            return true;
        }

        public void ResiveMessage(string massage)
        {}
    }
}