using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SituationCenterCore.Data;
using SituationCenterCore.Models.StorageModels;
using Storage.Interfaces;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SituationCenterCore.Controllers.API.V1
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v1/[controller]/[action]/{*pathToFolder}")]
    public class StorageController : Controller
    {
        private readonly IStorage storageManager;
        private readonly UserManager<ApplicationUser> userManager;
        private ILogger<StorageController> logger;

        public StorageController(IStorage storageManager, UserManager<ApplicationUser> userManager,
            ILogger<StorageController> logger)
        {
            this.storageManager = storageManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult DirectoryContent(string pathToFolder)
        {
            var userId = userManager.GetUserId(User);
            try
            {
                DirectoryContent content = new DirectoryContent
                {
                    Files =
                    storageManager
                    .GetDirectory("Moq token", userId, pathToFolder ?? "")
                    .Files.ToList()
                };
                return Json(content);
            }
            catch
            {
                return BadRequest();
            }
        }

        //[HttpGet]
        //public IActionResult Download(string pathToFolder)
        //{
        //    var userId = userManager.GetUserId(User);
        //    try
        //    {
        //        var stream = storageManager.GetFileStream(userId, pathToFolder);
        //        return File(stream, "application/octet-stream");
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpGet]
        //public IActionResult GetPicturesFor(string pathToFolder)
        //{
        //    var userId = userManager.GetUserId(User);
        //    try
        //    {
        //        var pictures = storageManager.GetPublicFileInfo(userId, pathToFolder).Pictures;
        //        foreach (var pic in pictures)
        //        {
        //            pic.Link = LinkToFile(userId, pic.Path);
        //        }
        //        return Json(new
        //        {
        //            Pictures = pictures
        //        });
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpGet]
        //public IActionResult GetLinkToFile(string pathToFolder)
        //{
        //    //return StatusCode(405);
        //    var userId = userManager.GetUserId(User);
        //    try
        //    {
        //        var file = storageManager.GetFileInfo(userId, pathToFolder ?? "");
        //        return Content(fileBuffer.ServLink + fileBuffer.GetLinkFor(file));
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }
        //}
    }
}