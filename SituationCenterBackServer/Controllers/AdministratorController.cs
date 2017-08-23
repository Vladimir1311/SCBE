using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SituationCenterBackServer.Data;
using SituationCenterBackServer.Extensions;
using SituationCenterBackServer.Logging;
using SituationCenterBackServer.Models;
using SituationCenterBackServer.Models.AdministratorViewModels;
using SituationCenterBackServer.Models.VoiceChatModels;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            ApplicationDbContext dataBase,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.roomsManager = roomsManager;
            this.dataBase = dataBase;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                Users = dataBase.Users
                .Include(U => U.Roles)
                .Include(U => U.Room)
                .ToList(),
                Rooms = dataBase
                .Rooms
                .Include(R => R.SecurityRule)
                .ToList(),
                Roles = roleManager.Roles.ToList()
            };
            return View(model);
        }

        public IActionResult Room(Guid roomId)
        {
            var room = roomsManager.FindRoom(roomId);
            if (room != null)
                return View(room);
            else
                return RedirectToAction("Index");
        }

        public async Task<IActionResult> Logs()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
                return View();
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await SocketLoggerProvider.AddSocketAsync(socket);
            return new EmptyResult();
        }

        public async Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId)
        {
            try
            {
                var user = await userManager.FindUser(User);
                var role = await roleManager.FindByIdAsync(roleId.ToString());
                userManager.RemoveFromRoleAsync(user, role.Name).Wait();
                var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
                if (usersInRole.Count == 0)
                    await roleManager.DeleteAsync(role);
            }
            catch
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteRoom(Guid roomId)
        {
            await Task.CompletedTask;
            roomsManager.DeleteRoom(userManager.GetUserGuid(User), roomId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            await userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public IActionResult Find()
        {
            var phones = new string[] { "+79161853166", "+78005553535" };
            var users = dataBase.Users.Where(U => phones.Contains(U.PhoneNumber)).ToArray();

            return Json(users);
        }
    }
}