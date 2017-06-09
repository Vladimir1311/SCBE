using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.StorageModels
{
    public interface IStorageManager
    {
        DirectoryContent GetContentInFolder(string ownerId, string pathToFolder);
        void Save(string userId, string pathToFolder, IFormFile fileToSave);
    }
}
