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
using System.Net.Http;
using IO = System.IO;
using SituationCenterBackServer.Services;

namespace SituationCenterBackServer.Controllers
{
    [Authorize]
    [Route("storage/[action]/{*pathToFolder}")]
    public class StorageController : Controller
    {
        private readonly IStorageManager storageManager;
        private readonly UserManager<ApplicationUser> userManager;
        private IDocumentHandlerService docHandler;

        public StorageController(IStorageManager storageManager,
            UserManager<ApplicationUser> userManager,
            IDocumentHandlerService docHandler)
        {
            this.storageManager = storageManager;
            this.userManager = userManager;
            this.docHandler = docHandler;
        }


        public IActionResult Index(string pathToFolder)
        {

            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();
            var content = storageManager.GetContentInFolder(userId, pathToFolder);
            docHandler.FillStates(content.Files);
            return View(content);
        }

        public IActionResult Add(string pathToFolder, IFormFile file)
        {
            pathToFolder = pathToFolder ?? "";
            string userId = GetUserId();
            File savedFile;
            if (docHandler.IsSupported(IO.Path.GetExtension(file.FileName)))
            {
                savedFile = storageManager.SaveDocument(userId, pathToFolder, file);
                docHandler.SendDocumentToHandle(savedFile);
            }
            else
            {
                savedFile = storageManager.Save(userId, pathToFolder, file);
            }
            //TODO handle errors with sendind doc!!!
            docHandler.FillState(savedFile);
            return RedirectToAction("index");
        }


        private string GetUserId()
        {
            return userManager.GetUserId(User);
        }
    }
}