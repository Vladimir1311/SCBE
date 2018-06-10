using Microsoft.Extensions.Options;
using SituationCenterBackServer.Interfaces;
using Storage.Interfaces;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Storage
{
    /// <summary>
    /// Сервис хранилища, поддерживающий работу с документами
    /// </summary>
    public class DocumentSupportStorage : SimpleStorage
    {
        private static readonly Regex DocExtensionRegex = new Regex(@"^.(docx?|pdf)$", RegexOptions.Multiline);

        protected readonly IDocumentPageManager DocumentPageManager;

        public DocumentSupportStorage(IOptions<StorageSetting> setting, IDocumentPageManager documentPageManager, IAccessValidator accessValidator, IFileSystem fileSystem)
            : base(setting, accessValidator, fileSystem)
        {
            DocumentPageManager = documentPageManager;
        }

        public override bool CreateFile(string Token, string Owner, string Path, Stream ContentStream)
        {
            if (!base.CreateFile(Token, Owner, Path, ContentStream))
            {
                return false;
            }

            if (IsDocument(Path))
            {
                DocumentPageManager.ProcessDocument(Owner, Path);
            }

            return true;
        }

        public override bool DeleteFile(string Token, string Owner, string Path)
        {
            if (!base.DeleteFile(Token, Owner, Path))
            {
                return false;
            }

            if (IsDocument(Path))
            {
                FileSystem.DeleteFile(GetRealMetaDir(Owner, Path));
            }
            return true;
        }

        protected override DirectoryDesc CollectDirInfo(string Path)
        {
            var desc = base.CollectDirInfo(Path);
            desc.Documents = desc.Files.Where(fi => IsDocument(fi.Name)).Select(fi =>
            {
                var meta = LoadMeta(FileSystem.CombinePath(Path, fi.Name));
                return new DocumentDesc
                {
                    Name = fi.Name,
                    Size = fi.Size,
                    FullPath = fi.FullPath,

                    PageCount = meta.PageCount,
                    ReadyCount = meta.PageCount - meta.NotReadyPage.Count
                };
            }).ToList();

            desc.Files = desc.Files.Where(fi => !IsDocument(fi.Name)).ToList();
            return desc;
        }

        public override Stream GetDocumentPageContent(string Token, string Owner, string Path, int Page)
        {
            if (!IsDocument(Path) || !CheckAccess(Token, Owner, Path))
                return null;

            return FileSystem.ReadInFile(GetRealPagePath(Owner, Path, Page));
        }

        public override Stream GetDocumentOverlayContent(string Token, string Owner, string Path, int Page)
        {
            if (!IsDocument(Path) || !CheckAccess(Token, Owner, Path))
                return null;

            return FileSystem.ReadInFile(GetRealOverlayPath(Owner, Path, Page));
        }

        public override void SetDocumentOverlayContent(string Token, string Owner, string Path, int Page, Stream Content)
        {
            if (!IsDocument(Path) || !CheckAccess(Token, Owner, Path))
                return;

            var real = GetRealOverlayPath(Owner, Path, Page);

            if (Content != null)
            {
                FileSystem.WriteToFile(real, Content);
            }
            else
            {
                FileSystem.DeleteFile(real);
            }
        }

        public override bool Move(string Token, string Owner, string OldPath, string NewPath)
        {
            if (!base.Move(Token, Owner, OldPath, NewPath))
            {
                return false;
            }

            if (IsDocument(OldPath))
            {
                FileSystem.MoveDirectory(GetRealMetaDir(Owner, OldPath), GetRealMetaDir(Owner, NewPath));
            }

            return true;
        }

        public override bool Copy(string Token, string Owner, string OldPath, string NewPath)
        {
            if (!base.Copy(Token, Owner, OldPath, NewPath))
            {
                return false;
            }

            if (IsDocument(OldPath))
            {
                FileSystem.CopyDirectory(GetRealMetaDir(Owner, OldPath), GetRealMetaDir(Owner, NewPath));
            }

            return true;
        }

        protected bool IsDocument(string path)
        {
            return DocExtensionRegex.IsMatch(FileSystem.GetFileExtension(path));
        }
    }
}