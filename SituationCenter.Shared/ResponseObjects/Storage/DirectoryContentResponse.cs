using Newtonsoft.Json;
using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationCenter.Shared.ResponseObjects.Storage
{
    public class DirectoryContentResponse : ResponseBase
    {
        [JsonProperty("directories")]
        public List<IDirectoryDesc> Directories { get; set; }
        [JsonProperty("files")]
        public List<IFileDesc> Files { get; set; }
        [JsonProperty("documents")]
        public List<IDocumentDesc> Documents { get; set; }

        protected DirectoryContentResponse(List<IDirectoryDesc> directories, List<IFileDesc> files, List<IDocumentDesc> docs) : base() =>
            (Directories, Files, Documents) = (directories, files, docs);

        public static DirectoryContentResponse Create(List<IDirectoryDesc> directories, List<IFileDesc> files, List<IDocumentDesc> docs) =>
            new DirectoryContentResponse(directories, files, docs);
    }
}
