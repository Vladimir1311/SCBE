using System.Collections.Generic;

namespace SituationCenterBackServer.Models.StorageModels
{
    public class DirectoryContent
    {
        public List<Directory> Directories { get; set; }
        public List<File> Files { get; set; }
    }
}