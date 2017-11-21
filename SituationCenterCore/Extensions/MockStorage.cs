using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace SituationCenterCore.Extensions
{
    public class MockStorage : IStorage
    {
        public bool Copy(string Token, string Owner, string OldPath, string NewPath)
        {
            throw new NotImplementedException();
        }

        public bool CreateDirectory(string Token, string Owner, string Path)
        {
            throw new NotImplementedException();
        }

        public bool CreateFile(string Token, string Owner, string Path, Stream ContentStream)
        {
            throw new NotImplementedException();
        }

        public bool DeleteDirectory(string Token, string Owner, string Path)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFile(string Token, string Owner, string Path)
        {
            throw new NotImplementedException();
        }

        public IDirectoryDesc GetDirectoryInfo(string Token, string Owner, string Path)
        {
            return new DirectoryDesc
            {
                Name = "Root",
                FullPath = "",
                Directories = new List<IDirectoryDesc>
                {
                    new DirectoryDesc
                    {
                        Name = "Ahaha",
                        FullPath = "Ahaha"
                    },
                    new DirectoryDesc
                    {
                        Name = "Ohoho",
                        FullPath = "Ohoho"
                    }
                },
                Files = new List<FileDesc>
                {
                    new FileDesc
                    {
                        Name = "pic.png",
                        FullPath = "pic.png"
                    }
                },
                Documents = Enumerable.Empty<IDocumentDesc>()
            };
        }
        class FileDesc : IFileDesc
        {
            public string Name { get; set; }

            public string FullPath { get; set; }

            public long Size { get; set; }
        }
        class DirectoryDesc : IDirectoryDesc
        {
            public string FullPath { get; set; }

            public string Name { get; set; }

            public IEnumerable<IFileDesc> Files { get; set; }

            public IEnumerable<IDocumentDesc> Documents { get; set; }

            public IEnumerable<IDirectoryDesc> Directories { get; set; }
        }


        public Stream GetDocumentOverlayContent(string Token, string Owner, string Path, int Page)
        {
            throw new NotImplementedException();
        }

        public Stream GetDocumentPageContent(string Token, string Owner, string Path, int Page)
        {
            throw new NotImplementedException();
        }

        public Stream GetFileContent(string Token, string Owner, string Path)
        {
            throw new NotImplementedException();
        }

        public IFileDesc GetFileInfo(string Token, string Owner, string Path)
        {
            throw new NotImplementedException();
        }

        public bool IsExistDirectory(string Token, string Owner, string Path)
        {
            throw new NotImplementedException();
        }

        public bool IsExistFile(string Token, string Owner, string Path)
        {
            throw new NotImplementedException();
        }

        public bool Move(string Token, string Owner, string OldPath, string NewPath)
        {
            throw new NotImplementedException();
        }

        public void SetDocumentOverlayContent(string Token, string Owner, string Path, int Page, Stream Content)
        {
            throw new NotImplementedException();
        }
    }
}
