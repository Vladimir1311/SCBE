using DocsToPictures.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models;
using SituationCenterBackServer.Models.StorageModels;
using SituationCenterBackServer.Services;
using Storage.Interfaces;
using System.Linq;
using System.Threading;
using IO = System.IO;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    [Route("datastorage/[action]/{*pathToFolder}")]
    public class DataStorageController : Controller
    {
        private readonly IStorage storageManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDocumentProccessor docsProcessor;

        public DataStorageController(IStorage storageManager,
            IDocumentProccessor docsProcessor,
            UserManager<ApplicationUser> userManager)
        {
            this.storageManager = storageManager;
            this.docsProcessor = docsProcessor;
            this.userManager = userManager;
        }

        public IActionResult Index(string pathToFolder)
        {
            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();

            var content = storageManager.GetDirectory("Moq token", userId, "");
            
            return View(new DirectoryContent { Directories = content.Directories.ToList(), Files = content.Files.ToList()});
        }

        public IActionResult Add(string pathToFolder, IFormFile file)
        {
            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();
            var doc = docsProcessor.AddToHandle(IO.Path.GetFileName(file.FileName), file.OpenReadStream());
            //var targetFolder = storageManager.GetDirectory("Moq token", userId, pathToFolder);
            //targetFolder.CreateFile(IO.Path.GetFileName(file.FileName), file.OpenReadStream());
            int i = 20;
            while (i > 0)
            {
                System.Console.WriteLine(i);
                Thread.Sleep(1000);
                i--;
            }
            return RedirectToAction("index");
        }

        private string GetUserId()
        {
            return userManager.GetUserId(User);
        }
    }
}