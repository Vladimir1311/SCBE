using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationCenterBackServer.Models.StorageModels
{
    public class DirectoryContent
    {
        public IEnumerable<Directory> Directories { get; set; }
        public IEnumerable<File> Files { get; set; }
    }
}
