using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models.StorageModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SituationCenterBackServer.Models;
using Microsoft.AspNetCore.Http;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    [Route("storage/[action]/{*pathToFolder}")]
    public class StorageController : Controller
    {
        private readonly IStorageManager storageManager;
        private readonly UserManager<ApplicationUser> userManager;

        public StorageController(IStorageManager storageManager, UserManager<ApplicationUser> userManager)
        {
            this.storageManager = storageManager;
            this.userManager = userManager;
        }


        public IActionResult Index(string pathToFolder)
        {

            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();
            return View(storageManager.GetContentInFolder(userId, pathToFolder));
        }

        public IActionResult Add(string pathToFolder, IFormFile file)
        {
            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();
            storageManager.Save(userId, pathToFolder, file);
            return RedirectToAction("index");
        }


        private string GetUserId()
        {
            return userManager.GetUserId(User);
        }
    }
}