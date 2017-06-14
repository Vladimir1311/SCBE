using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SituationCenterBackServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        public File Save(string userId, string pathToFolder, IFormFile file)
        {
            var folderPath = GetDirectoryPathIfCorrect(userId, pathToFolder);
            var fileName = NameFromPath(file.FileName);
            var filePath = IO.Path.Combine(folderPath, fileName);
            using (var filestream = IO.File.Create(filePath))
            {
                file.CopyTo(filestream);
            }
            return GetFileInfo(filePath);
        }
        public File SaveDocument(string userId, string pathToFolder, IFormFile file)
        {
            var folderPath = GetDirectoryPathIfCorrect(userId, pathToFolder);
            var fileName = NameFromPath(file.FileName);
            var fileFolderPath = IO.Path.Combine(folderPath, fileName);
            IO.Directory.CreateDirectory(fileFolderPath);
            var filePath = IO.Path.Combine(fileFolderPath, fileName);
            using (var filestream = IO.File.Create(filePath))
            {
                file.CopyTo(filestream);
            }
            return GetDocumentInfo(filePath);
        }
        public IO.Stream GetFileStream(string localPath)
        {
            var targetPath = IO.Path.Combine(storageRoot, localPath);
            if (IO.Directory.Exists(targetPath))
            {
                var docName = NameFromPath(localPath);
                return IO.File.OpenRead(
                    IO.Path.Combine(targetPath, docName));
                   
            }
            else
                return IO.File.OpenRead(targetPath);
        }

        private DirectoryContent GetContent(string path)
        {
            var folders = IO.Directory.GetDirectories(path)
                .Select(P => (name: NameFromPath(P), path: P))
                .ToLookup(N => N.name.Contains('.'));

            return new DirectoryContent
            {
                Directories = folders[false]
                    .Select(N => new Directory { Name = N.name })
                    .ToList(),

                Files = IO.Directory.GetFiles(path)
                    .Concat(folders[true]
                            .Select(P => P.path))
                    .Select(GetFileInfo)
                    .ToList()
            };
        }

        private File GetDocumentInfo(string pathToDoc)
        {
            pathToDoc = pathToDoc.Substring(0, pathToDoc.LastIndexOf('\\'));
            var file = GetFileInfo(pathToDoc);
            var pictures = IO.Directory.GetFiles(pathToDoc)
                .Select(P => new Picture { Name = NameFromPath(P), Path = P, State = PictureState.Ready })
                .Where(P => Regex.IsMatch(P.Name, @"\d+\..*"))
                .Select(P => new { Picture = P, Number = int.Parse(Regex.Match(P.Name, @"\d+").Value) });
            foreach (var picObj in pictures)
                file.Pictures.AnyInsert(picObj.Number, picObj.Picture);
            return file;
        }

        private File GetFileInfo(string pathToFile)
        {
            var localPath = LocalPath(pathToFile);
            return new File()
            {
                Name = NameFromPath(pathToFile),
                State = FileReadyState.Unknow,
                Path = localPath
            };
        }

        private string LocalPath(string absolutePath)
        {
            string localPath = absolutePath.Substring(absolutePath.IndexOf("ForStorageFolder"));
            localPath = localPath.Substring("ForStorageFolder\\".Length);
            
            return localPath;
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
