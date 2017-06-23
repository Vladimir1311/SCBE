﻿using System;
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
using SituationCenterBackServer.Services;

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
        private IBuffer fileBuffer;

        public StorageController(IStorageManager storageManager, UserManager<ApplicationUser> userManager,
            ILogger<StorageController> logger,
            IBuffer fileBuffer)
        {
            this.storageManager = storageManager;
            this.userManager = userManager;
            this.logger = logger;
            this.fileBuffer = fileBuffer;
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
        public IActionResult Download(string pathToFile)
        {
            var userId = userManager.FindByNameAsync(userManager.GetUserName(User)).Result.Id;
            try
            {
                var stream = storageManager.GetFileStream(userId, pathToFile);
                return File(stream, "application/octet-stream");
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


        [HttpGet]
        public IActionResult GetLinkToFile(string pathToFolder)
        {
            var userId = userManager.FindByNameAsync(userManager.GetUserName(User)).Result.Id;
            try
            {
                var file = storageManager.GetFileInfo(userId, pathToFolder ?? "");
                return Content(fileBuffer.ServLink + fileBuffer.GetLinkFor(file));
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
