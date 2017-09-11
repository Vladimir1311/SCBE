using Newtonsoft.Json;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterCore.Models.StorageModels
{
    public class DirectoryContent
    {
        [JsonProperty("directories")]
        public List<IDirectory> Directories { get; set; }
        [JsonProperty("files")]
        public List<IFile> Files { get; set; }
    }
}
