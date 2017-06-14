using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.StorageModels
{
    public interface IStorageManager
    {
        DirectoryContent GetContentInFolder(string ownerId, string pathToFolder);
        File Save(string userId, string pathToFolder, IFormFile fileToSave);
        Stream GetFileStream(string localPath);
        File SaveDocument(string userId, string pathToFolder, IFormFile file);
        Task SavePictureAsync(File file, Picture pic, Stream stream);
    }
}
