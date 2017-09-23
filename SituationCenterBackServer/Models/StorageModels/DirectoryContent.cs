using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Storage.Interfaces;
using System.Collections.Generic;

namespace SituationCenterBackServer.Models.StorageModels
{
    public class DirectoryContent
    {
        [JsonProperty("directories")]
        public List<IDirectory> Directories { get; set; }
        [JsonProperty("files")]
        public List<IFile> Files { get; set; }
    }
}