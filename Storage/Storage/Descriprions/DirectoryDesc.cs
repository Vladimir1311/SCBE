using Storage.Interfaces;
using System.Collections.Generic;

namespace Storage
{
    public class DirectoryDesc : IDirectoryDesc
    {
        public string FullPath { get; set; }
        public string Name { get; set; }

        public IEnumerable<IFileDesc> Files { get; set; }
        public IEnumerable<IDocumentDesc> Documents { get; set; }
        public IEnumerable<IDirectoryDesc> Directories { get; set; }
    }
}