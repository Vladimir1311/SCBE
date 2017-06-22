using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SituationCenterBackServer.Models.StorageModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SituationCenterBackServer.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Features;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SituationCenterBackServer.Controllers.API.V1
{
    [Authorize]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    public class StorageController : Controller
    {
        private readonly IStorageManager storageManager;
        private readonly UserManager<ApplicationUser> userManager;
        private ILogger<StorageController> logger;

        public StorageController(IStorageManager storageManager, UserManager<ApplicationUser> userManager,
            ILogger<StorageController> logger)
        {
            this.storageManager = storageManager;
            this.userManager = userManager;
            this.logger = logger;
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
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Download(string pathToFolder)
        {
            //var userId = userManager.FindByNameAsync(userManager.GetUserName(User)).Result.Id;
            var userId = "8bbf0059-72d1-4ddc-a748-97995d183d52";//TODO БЛЯТЬ ТАК НЕЛЬЗЯ ДЕЛАТЬ
            try
            {
                foreach (var feature in HttpContext.Features)
                    logger.LogInformation($"feature type {feature.Key.FullName} value {feature.Value.ToString()}");
                return PhysicalFile(@"D:\WindowsLibrares\Documents\Visual Studio 2017\Projects\SituationCenterBackServer\SituationCenterBackServer\AppData\ForStorageFolder\8bbf0059-72d1-4ddc-a748-97995d183d52\Belyaeva_Olya_k_pr.doc\1.png", "application/octet-stream");
            }
            catch
            {
                return BadRequest();
            }
        }
        
        [HttpGet]
        public IActionResult GetPicturesFor(string pathToFolder)
        {
            var userId = userManager.FindByNameAsync(userManager.GetUserName(User)).Result.Id;
            try
            {
                var pictures = storageManager.GetPublicFileInfo(userId, pathToFolder).Pictures;
                return Json(new {
                    Pictures = pictures
                });
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
