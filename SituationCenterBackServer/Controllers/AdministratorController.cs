using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SituationCenterBackServer.Logging;
using SituationCenterBackServer.Models.VoiceChatModels;

namespace SituationCenterBackServer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : Controller
    {
        private IRoomManager _roomsManager;

        public AdministratorController(IRoomManager roomsManager)
        {
            _roomsManager = roomsManager;
        }
        public IActionResult Index()
        {
            return View();
            
        }

        public async Task<IActionResult> Rooms()
        {
            return View(_roomsManager.Rooms);
        }

        public async Task<IActionResult> Logs()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
                return View();
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await SocketLoggerProvider.AddSocketAsync(socket);
            return new EmptyResult();
        }
    }
}