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
        public List<IDirectoryDesc> Directories { get; set; }
        [JsonProperty("files")]
        public List<IFileDesc> Files { get; set; }
    }
}
