using Microsoft.Extensions.Options;
using SituationCenterBackServer.Interfaces;
using Storage.Interfaces;
using System.IO;
using System.Linq;

namespace Storage
{
    /// <summary>
    /// Сервис хранилища, поддерживающий работу файлами и папками
    /// </summary>
    public class SimpleStorage : StorageComponent, IStorage
    {
        protected readonly IAccessValidator AccessValidator;

        protected SimpleStorage(IOptions<StorageSetting> setting, IAccessValidator accessValidator, IFileSystem fileSystem)
            : base(setting, fileSystem)
        {
            AccessValidator = accessValidator;
        }

        public bool CreateDirectory(string Token, string Owner, string Path)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return false;

            FileSystem.CreateDirectory(real);
            return true;
        }

        public virtual bool CreateFile(string Token, string Owner, string Path, Stream ContentStream)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return false;

            using (var fs = FileSystem.CreateFile(real))
            {
                ContentStream.CopyTo(fs);
            }

            return true;
        }

        public IDirectoryDesc GetDirectoryInfo(string Token, string Owner, string Path)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return null;

            if (!FileSystem.DirectoryExists(real))
                return null;

            return CollectDirInfo(real);
        }

        public IFileDesc GetFileInfo(string Token, string Owner, string Path)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return null;

            if (!FileSystem.FileExists(real))
                return null;

            return CollectFileInfo(real);
        }

        protected virtual DirectoryDesc CollectDirInfo(string Path)
        {
            return new DirectoryDesc
            {
                Name = FileSystem.GetFileName(Path),
                FullPath = GetRelativePath(Path),

                Files = FileSystem.GetFiles(Path).Select(x => FileSystem.CombinePath(Path, x)).Select(CollectFileInfo).ToList(),
                Directories = FileSystem.GetDirectories(Path).Select(x => FileSystem.CombinePath(Path, x)).Select(CollectDirInfo).ToList()
            };
        }

        protected FileDesc CollectFileInfo(string Path)
        {
            return new FileDesc
            {
                Name = FileSystem.GetFileName(Path),
                Size = FileSystem.GetFileSize(Path),
                FullPath = GetRelativePath(Path),
            };
        }

        public Stream GetFileContent(string Token, string Owner, string Path)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return null;

            if (!FileSystem.FileExists(real))
                return null;

            return FileSystem.ReadInFile(real);
        }

        public bool DeleteDirectory(string Token, string Owner, string Path)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return false;

            if (!FileSystem.DirectoryExists(Path))
                return false;

            FileSystem.DeleteDirectory(real, true);
            return true;
        }

        public virtual bool DeleteFile(string Token, string Owner, string Path)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return false;

            if (!FileSystem.FileExists(Path))
                return false;

            FileSystem.DeleteFile(real);
            return true;
        }

        public bool IsExistFile(string Token, string Owner, string Path)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return false;

            return FileSystem.FileExists(Path);
        }

        public bool IsExistDirectory(string Token, string Owner, string Path)
        {
            if (!CheckAndGetRealPath(Token, Owner, Path, out var real))
                return false;

            return FileSystem.DirectoryExists(Path);
        }

        public virtual bool Move(string Token, string Owner, string OldPath, string NewPath)
        {
            if (!CheckAndGetRealPath(Token, Owner, OldPath, out var realOld))
                return false;

            var realNew = GetRealPath(Owner, NewPath);

            if (FileSystem.FileExists(OldPath) || !FileSystem.FileExists(realNew))
            {
                FileSystem.MoveFile(realOld, realNew);
                return true;
            }

            if (FileSystem.DirectoryExists(OldPath) || !FileSystem.DirectoryExists(realNew))
            {
                FileSystem.CopyDirectory(realOld, realNew);
                return true;
            }

            return false;
        }

        public virtual bool Copy(string Token, string Owner, string OldPath, string NewPath)
        {
            if (!CheckAndGetRealPath(Token, Owner, OldPath, out var realOld))
                return false;

            var realNew = GetRealPath(Owner, NewPath);

            if (FileSystem.FileExists(OldPath) || !FileSystem.FileExists(realNew))
            {
                FileSystem.CopyFile(realOld, realNew);
                return true;
            }

            if (FileSystem.DirectoryExists(OldPath) || !FileSystem.DirectoryExists(realNew))
            {
                FileSystem.CopyDirectory(realOld, realNew);
                return true;
            }

            return false;
        }

        public virtual Stream GetDocumentPageContent(string Token, string Owner, string Path, int Page)
        {
            return null;
        }

        public virtual Stream GetDocumentOverlayContent(string Token, string Owner, string Path, int Page)
        {
            return null;
        }

        public virtual void SetDocumentOverlayContent(string Token, string Owner, string Path, int Page, Stream Content)
        {
        }

        protected bool CheckAccess(string Token, string Owner, string path)
        {
            var real = FileSystem.CombinePath(Owner, path);
            return AccessValidator.CanAccessToFolder(Token, real);
        }

        protected bool CheckAndGetRealPath(string Token, string Owner, string path, out string RealPath)
        {
            if (!CheckAccess(Token, Owner, path))
            {
                RealPath = null;
                return false;
            }

            RealPath = GetRealPath(Owner, path);

            if ((path == "" || path == ".") && !FileSystem.DirectoryExists(RealPath))
            {
                FileSystem.CreateDirectory(RealPath);
            }

            return true;
        }
    }
}