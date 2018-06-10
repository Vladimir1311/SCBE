using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Storage.Interfaces;

namespace Storage
{
    public class StorageComponent
    {
        protected readonly StorageSetting Setting;
        protected readonly IFileSystem FileSystem;

        internal StorageComponent(IOptions<StorageSetting> setting, IFileSystem fileSystem)
        {
            Setting = setting.Value;
            FileSystem = fileSystem;
        }

        protected string GetRealPath(string Owner, string path)
        {
            return FileSystem.CombinePath(Setting.PathToUserSpaces, Owner, path);
        }

        protected string GetRealMetaDir(string Owner, string path)
        {
            return FileSystem.CombinePath(Setting.PathToDocumentSpaces, Owner, path);
        }

        protected string GetRealPagePath(string Owner, string path, int pageIndex)
        {
            return FileSystem.CombinePath(GetRealMetaDir(Owner, path), pageIndex + Setting.DocumentMetaPageExtension);
        }

        protected string GetRealOverlayPath(string Owner, string path, int pageIndex)
        {
            return FileSystem.CombinePath(GetRealMetaDir(Owner, path), pageIndex + Setting.DocumentPaintExtension);
        }

        protected string GetRealMetaFilePath(string Owner, string path)
        {
            return FileSystem.CombinePath(GetRealMetaDir(Owner, path), Setting.DocumentMetaFileName);
        }

        protected string GetRelativePath(string path)
        {
            var Base = FileSystem.NormalizePath(Setting.PathToUserSpaces);
            var real = FileSystem.NormalizePath(path);

            if (real.StartsWith(Base))
            {
                real = real.Substring(Base.Length + 1);

                if (real.Length != 0)
                    real = real.Substring(real.IndexOf('\\') + 1);
            }

            return real;
        }

        protected void SaveMeta(string Owner, string Path, DocumentMeta Meta)
        {
            var real = GetRealMetaFilePath(Owner, Path);

            FileSystem.WriteAllTextToFile(real, JsonConvert.SerializeObject(Meta));
        }

        protected DocumentMeta LoadMeta(string Owner, string Path)
        {
            var real = GetRealMetaFilePath(Owner, Path);

            return JsonConvert.DeserializeObject<DocumentMeta>(FileSystem.ReadAllTextInFile(real));
        }

        protected DocumentMeta LoadMeta(string path)
        {
            if (path.StartsWith(Setting.PathToUserSpaces))
            {
                path = Setting.PathToDocumentSpaces + path.Substring(Setting.PathToUserSpaces.Length);
            }

            var content = FileSystem.ReadAllTextInFile(Path.Combine(path, Setting.DocumentMetaFileName));
            return JsonConvert.DeserializeObject<DocumentMeta>(content);
        }
    }
}