using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models.StorageModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SituationCenterBackServer.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SituationCenterBackServer.Controllers.API.V1
{
    [Authorize]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    public class StorageController : Controller
    {
        private readonly IStorageManager storageManager;
        private readonly UserManager<ApplicationUser> userManager;

        public StorageController(IStorageManager storageManager, UserManager<ApplicationUser> userManager)
        {
            this.storageManager = storageManager;
            this.userManager = userManager;

        }
        [HttpGet]
        public IActionResult DirectoryContent(string pathToFolder)
        {
            var userId = userManager.FindByNameAsync(userManager.GetUserName(User)).Result.Id;
            try
            {           
                return Json(storageManager.GetPublicContentInFolder(userId, pathToFolder ?? ""));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Download(string pathToFolder)
        {
            var userId = userManager.FindByNameAsync(userManager.GetUserName(User)).Result.Id;
            try
            {
                return File(storageManager.GetFileStream(userId, pathToFolder ?? ""), "application/octet-stream", "file.png");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
