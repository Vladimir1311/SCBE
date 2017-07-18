using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SituationCenterBackServer.Logging;
using SituationCenterBackServer.Models.VoiceChatModels;
using SituationCenterBackServer.Data;
using SituationCenterBackServer.Models.AdministratorViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SituationCenterBackServer.Models;
using Microsoft.EntityFrameworkCore;

namespace SituationCenterBackServer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : Controller
    {
        private IRoomManager roomsManager;
        private readonly ApplicationDbContext dataBase;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministratorController(IRoomManager roomsManager,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.roomsManager = roomsManager;
            this.dataBase = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                Users = userManager.Users.Include(U => U.Roles).ToList(),
                Rooms = roomsManager.Rooms.ToList(),
                Roles = roleManager.Roles.ToList()
            };
            return View(model);
            
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