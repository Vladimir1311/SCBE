using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace UDPServerTester.Controllers
{
    public class UDPController : Controller
    {
        private static UdpConnector _connector;
        private ILogger<UDPController> _logger;

        public UDPController(ILogger<UDPController> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            if (_connector == null)
            {
                _connector = new UdpConnector(13000, loggerFactory.CreateLogger<UdpConnector>());
                _connector.Start();
                _logger.LogInformation("Connector Started!");
            }
        }
        public IActionResult Index()
        {
            return View();
            
        }


    }
}