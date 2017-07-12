using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models.VoiceChatModels;
using Microsoft.AspNetCore.Authorization;
using SituationCenterBackServer.Data;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    public class AdministratorController : Controller
    {
        private readonly ApplicationDbContext dataBase;

        public AdministratorController(ApplicationDbContext dataBase)
        {
            this.dataBase= dataBase;
        }
        public IActionResult Index()
        {
            return View(dataBase.Users);
        }
    }
}