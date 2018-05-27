using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Storage.Interfaces;

namespace Storage.Service.Models
{
    public class DirectoryContent
    {
        [JsonProperty("directories")]
        public List<IDirectoryDesc> Directories { get; set; }

        [JsonProperty("Documents")]
        public List<IDocumentDesc> Documents { get; set; }

        [JsonProperty("files")]
        public List<IFileDesc> Files { get; set; }
    }
}
