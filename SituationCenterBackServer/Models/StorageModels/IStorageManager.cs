﻿using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.StorageModels
{
    public interface IStorageManager
    {
        DirectoryContent GetContentInFolder(string ownerId, string pathToFolder);

        DirectoryContent GetPublicContentInFolder(string ownerId, string pathToFolder);

        File Save(string userId, string pathToFolder, IFormFile fileToSave);

        Stream GetFileStream(string localPath);

        Stream GetFileStream(string ownerId, string pathToFile);

        File GetFileInfo(string ownerId, string pathToFile);

        File GetPublicFileInfo(string ownerId, string pathToFile);

        File SaveDocument(string userId, string pathToFolder, IFormFile file);

        Task SavePictureAsync(File file, Picture pic, Stream stream);
    }
}