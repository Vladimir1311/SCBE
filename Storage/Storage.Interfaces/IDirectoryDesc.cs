using System.Collections.Generic;

namespace Storage.Interfaces
{
    public interface IDirectoryDesc
    {
        string FullPath { get; }
        string Name { get; }

        IEnumerable<IFileDesc> Files { get; }
        IEnumerable<IDocumentDesc> Documents { get; }
        IEnumerable<IDirectoryDesc> Directories { get; }
    }
}