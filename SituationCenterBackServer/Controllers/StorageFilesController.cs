using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models;
using SituationCenterBackServer.Models.StorageModels;
using SituationCenterBackServer.Services;
using Storage.Interfaces;
using System.Linq;
using IO = System.IO;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    [Route("storagefiles/[action]/{*pathToFolder}")]
    public class StorageFilesController : Controller
    {
        private readonly IStorage storageManager;
        private readonly UserManager<ApplicationUser> userManager;

        public StorageFilesController(IStorage storageManager,
            UserManager<ApplicationUser> userManager)
        {
            this.storageManager = storageManager;
            this.userManager = userManager;
        }

        public IActionResult Index(string pathToFolder)
        {
            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();

            if (!storageManager.ExistsUserSpace(userId))
                storageManager.CreateUserSpace(userId);

            var content = storageManager.GetRootDirectory(userId).GetDirectory(pathToFolder);
            
            return View(new DirectoryContent { Directories = content.Directories.ToList(), Files = content.Files.ToList()});
        }

        public IActionResult Add(string pathToFolder, IFormFile file)
        {
            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();

            if (!storageManager.ExistsUserSpace(userId))
                storageManager.CreateUserSpace(userId);

            var targetFolder = storageManager.GetRootDirectory(userId).GetDirectory(pathToFolder);
            targetFolder.CreateFile(file.FileName, file.OpenReadStream());
            return RedirectToAction("index");
        }

        private string GetUserId()
        {
            return userManager.GetUserId(User);
        }
    }
}