using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO = System.IO;

namespace SituationCenterBackServer.Models.StorageModels
{
    public class InProjectSavingStorageManager : IStorageManager
    {
        private readonly static string storageRoot;
        private readonly ILogger<InProjectSavingStorageManager> logger;

        static InProjectSavingStorageManager()
        {
            storageRoot = IO.Path.Combine(IO.Directory.GetCurrentDirectory(), "AppData", "ForStorageFolder");
        }
        public InProjectSavingStorageManager(ILogger<InProjectSavingStorageManager> logger)
        {
            this.logger = logger;
        }
        public DirectoryContent GetContentInFolder(string ownerId, string pathToFolder)
        {
            var wantedPath = GetDirectoryPathIfCorrect(ownerId, pathToFolder);
            return GetContent(wantedPath);
        }

        public void Save(string userId, string pathToFolder, IFormFile file)
        {
            var folderPath = GetDirectoryPathIfCorrect(userId, pathToFolder);
            var fileName = NameFromPath(file.FileName);
            var filePath = IO.Path.Combine(folderPath, fileName);
            using (var filestream = IO.File.Create(filePath))
            {
                file.CopyTo(filestream);
            }
        }

        private DirectoryContent GetContent(string path)
        {
            return new DirectoryContent
            {
                Directories = IO.Directory.GetDirectories(path)
                  .Select(P => new Directory { Name = NameFromPath(P) }),
                Files = IO.Directory.GetFiles(path)
                    .Select(P => new File { Name = NameFromPath(P)})
            };
        }

        private string NameFromPath(string path)
        {
            return path.Substring(path.LastIndexOf('\\') + 1);
        }

        private string GetDirectoryPathIfCorrect(string userId, string path)
        {
            string pathToUserFolder = GetPathToUserFolder(userId);
            string wantedPath = IO.Path.Combine(pathToUserFolder, path);
            if (!IO.Directory.Exists(wantedPath))
            {
                logger.LogWarning($"directory {wantedPath} is not exist");
                throw new Exception("This directory not exist");
            }
            return wantedPath;
        }

        private string GetPathToUserFolder(string userId)
        {
            string wantedPath = IO.Path.Combine(storageRoot, userId);
            if (!IO.Directory.Exists(wantedPath))
                IO.Directory.CreateDirectory(wantedPath);
            return wantedPath;
        }
    }
}
