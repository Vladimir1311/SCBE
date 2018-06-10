using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocsToPictures.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage.Interfaces;
using Storage.Service.Models;
using URSA.Respose;

namespace Storage.Service.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    public class StorageController : Controller
    {
        private readonly IStorage storageManager;
        private ILogger<StorageController> logger;

        public StorageController(IStorage storageManager, ILogger<StorageController> logger)
        {
            this.storageManager = storageManager;
            this.logger = logger;
        }

        [HttpGet]
        [Route("{Owner}/{*path}")]
        public URespose<DirectoryContent> DirectoryContent(string Owner, string path)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                var directoryDescription = storageManager
                    .GetDirectoryInfo(token, ownerToken, path ?? "");

                var content = new DirectoryContent
                {
                    Files = directoryDescription.Files.ToList(),
                    Documents = directoryDescription.Documents.ToList(),
                    Directories = directoryDescription.Directories.ToList()
                };
                
                return content;
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}")]
        public URespose<bool> DeleteFile(string Owner, string path)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);
                
                return storageManager.DeleteFile(token, ownerToken, path);
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}")]
        public URespose<bool> DeleteDirectory(string Owner, string path)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                return storageManager.DeleteDirectory(token, ownerToken, path);
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}")]
        public URespose<bool> IsExistFile(string Owner, string path)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                return storageManager.IsExistFile(token, ownerToken, path);
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}")]
        public URespose<bool> IsExistDirectory(string Owner, string path)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                return storageManager.IsExistDirectory(token, ownerToken, path);
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}")]
        public URespose<bool> CreateDirectory(string Owner, string path)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                return storageManager.CreateDirectory(token, ownerToken, path);
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpPost]
        [Route("{Owner}/{path}")]
        public URespose LoadFile(string Owner, string path, IFormFile uploadedFile)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                if (storageManager.CreateFile(token, ownerToken, path, uploadedFile.OpenReadStream()))
                {
                    return URespose.GoodResponse();
                }
                return URespose.BadResponse();
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}")]
        public UCustomRespose<FileStreamResult> GetFileContent(string Owner, string path)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                var stream = storageManager.GetFileContent(token, ownerToken, path);

                if (stream == null)
                    return URespose.BadResponse();

                var fResult = File(stream, "application/octet-stream");
                return UCustomRespose<FileStreamResult>.Create(fResult);
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}/{Page}")]
        public UCustomRespose<FileStreamResult> GetDocumentPage(string Owner, string path, int Page)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                var stream = storageManager.GetDocumentPageContent(token, ownerToken, path, Page);

                if (stream == null)
                    return URespose.BadResponse();

                var fResult = File(stream, "application/octet-stream");
                return UCustomRespose<FileStreamResult>.Create(fResult);
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}/{Page}")]
        public UCustomRespose<FileStreamResult> GetDocumentOverlay(string Owner, string path, int Page)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                var stream = storageManager.GetDocumentOverlayContent(token, ownerToken, path, Page);

                if (stream == null)
                    return URespose.BadResponse();

                var fResult = File(stream, "application/octet-stream");
                return UCustomRespose<FileStreamResult>.Create(fResult);
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpPost]
        [Route("{Owner}/{path}/{Page}")]
        public URespose SetDocumentOverlay(string Owner, string path, int Page, IFormFile Content)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                using (var sm = Content.OpenReadStream())
                    storageManager.SetDocumentOverlayContent(token, ownerToken, path, Page, sm);

                return URespose.GoodResponse();
            }
            catch
            {
                return URespose.BadResponse();
            }
        }

        [HttpGet]
        [Route("{Owner}/{path}/{Page}")]
        public URespose RemoveDocumentOverlay(string Owner, string path, int Page)
        {
            try
            {
                var (token, ownerToken) = StandartHeader(Owner, ref path);

                storageManager.SetDocumentOverlayContent(token, ownerToken, path, Page, null);
                return URespose.GoodResponse();
            }
            catch
            {
                return URespose.BadResponse();
            }
        }
        
        private (string, string) GetTokens(string owner)
        {
            HttpContext.Request.Headers.TryGetValue("Authorization", out var tokens);

            var token = tokens.First().Split(' ').Last();

            if (owner == "self")
            {
                return (token, token);
            }

            return (token, owner);
        }

        private (string, string) StandartHeader(string owner, ref string path)
        {
            path = Uri.UnescapeDataString(path ?? ".");
            return GetTokens(owner);
        }
    }
}
