using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models.VoiceChatModels;
using Microsoft.AspNetCore.Authorization;

namespace SituationCenterBackServer.Controllers
{
    [Authorize(Roles = "")]
    public class AdministratorController : Controller
    {
        private readonly IRoomManager roomsManager;

        public AdministratorController(IRoomManager roomsManager)
        {
            this.roomsManager = roomsManager;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}