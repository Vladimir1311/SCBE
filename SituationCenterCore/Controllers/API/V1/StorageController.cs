using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SituationCenter.Shared.ResponseObjects;
using SituationCenter.Shared.ResponseObjects.Storage;
using SituationCenterCore.Data;
using Storage.Interfaces;
using System.Linq;
using SituationCenter.Shared.Exceptions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SituationCenterCore.Controllers.API.V1
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/v1/[controller]/[action]")]
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
        [Route("{*pathToFolder}")]
        public DirectoryContentResponse DirectoryContent(string pathToFolder)
        {
            var userId = userManager.GetUserId(User);
            try
            {
                var directoryDescription = storageManager
                    .GetDirectoryInfo("Moq token", userId, pathToFolder ?? "");
                var content = DirectoryContentResponse.Create
                (
                    directoryDescription.Directories.ToList(),
                    directoryDescription.Files.ToList(),
                    directoryDescription.Documents.ToList()
                );
                return content;
            }
            catch
            {
                throw new StatusCodeException(
                    SituationCenter.Shared.Exceptions.StatusCode.UnknownError);
            }
        }

        [HttpGet]
        public IActionResult DownloadPage(string pathToFile, int pageNum)
        {
            var userId = userManager.GetUserId(User);
            try
            {
                var (owner, path) = FillFields(pathToFile);
                var stream = storageManager.GetDocumentPageContent("some token", userId, path, pageNum);
                return File(stream, "application/octet-stream");
            }
            catch
            {
                return BadRequest();
            }
        }

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


        private (string owner, string path) FillFields(string folderPath)
        {
            var owner = folderPath.Substring(0, folderPath.IndexOf("/") == -1 ? folderPath.Length : folderPath.IndexOf("/"));
            var path = folderPath.Substring(owner.Length);
            owner = owner == "self" ? userManager.GetUserId(User) : owner;
            return (owner, path);
        }
    }
}