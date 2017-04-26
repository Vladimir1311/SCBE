using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SituationCenterBackServer.Logging;
using Microsoft.AspNetCore.Authorization;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    public class LogsController : Controller
    {
        ILogger<LogsController> _logger;
        public LogsController(ILogger<LogsController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Connect()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
                return Content("Адрес для WebSocket подключения");
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await SocketLoggerProvider.AddSocketAsync(socket);
            return new EmptyResult();
        }
    }
}