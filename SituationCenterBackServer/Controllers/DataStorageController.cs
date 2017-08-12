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
    public class DataStorageController : Controller
    {
        private readonly IStorage storageManager;
        private readonly UserManager<ApplicationUser> userManager;

        public DataStorageController(IStorage storageManager,
            UserManager<ApplicationUser> userManager)
        {
            this.storageManager = storageManager;
            this.userManager = userManager;
        }

        public IActionResult Index(string pathToFolder)
        {
            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();

            var content = storageManager.GetRootDirectory("Moq token", userId);
            
            return View(new DirectoryContent { Directories = content.Directories.ToList(), Files = content.Files.ToList()});
        }

        public IActionResult Add(string pathToFolder, IFormFile file)
        {
            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();

            var targetFolder = storageManager.GetDirectory("Moq token", userId, pathToFolder);
            targetFolder.CreateFile(file.FileName, file.OpenReadStream());
            return RedirectToAction("index");
        }

        private string GetUserId()
        {
            return userManager.GetUserId(User);
        }
    }
}