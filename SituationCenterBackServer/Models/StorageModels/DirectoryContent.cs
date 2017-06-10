using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.StorageModels
{
    public class DirectoryContent
    {
        public List<Directory> Directories { get; set; }
        public List<File> Files { get; set; }
    }
}
