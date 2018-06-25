using Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Storage
{
    public class RealFileSystem : IFileSystem
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            var full = Path.GetFullPath(path);

            return Directory.GetDirectories(full).Select(p => p.Substring(full.Length + 1));
        }

        public IEnumerable<string> GetFiles(string path)
        {
            var full = Path.GetFullPath(path);

            return Directory.GetFiles(full).Select(p => p.Substring(full.Length + 1));
        }

        public long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        public Stream CreateFile(string path)
        {
            return File.Create(path);
        }

        public Stream ReadInFile(string path)
        {
            var content = File.ReadAllBytes(path);
            return new MemoryStream(content);
        }

        public void WriteToFile(string path, Stream Content)
        {
            using (var fs = File.OpenWrite(path))
            {
                Content.CopyTo(fs);
            }
        }

        public string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public string NormalizePath(string path)
        {
            return Path.GetFullPath(path).Substring(Path.GetFullPath(".").Length).TrimStart('\\');
        }

        public string GetFileExtension(string path)
        {
            return new FileInfo(path).Extension;
        }

        public string GetFileName(string path)
        {
            return new FileInfo(path).Name;
        }

        public string ReadAllTextInFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteDirectory(string path, bool Recursive)
        {
            Directory.Delete(path, Recursive);
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public void MoveFile(string OldPath, string NewPath)
        {
            File.Move(OldPath, NewPath);
        }

        public void WriteAllTextToFile(string path, string Content)
        {
            File.WriteAllText(path, Content);
        }

        public void MoveDirectory(string OldPath, string NewPath)
        {
            Directory.Move(OldPath, NewPath);
        }

        public void CopyFile(string OldPath, string NewPath)
        {
            File.Copy(OldPath, NewPath, true);
        }

        public void CopyDirectory(string OldPath, string NewPath)
        {
            DirectoryInfo dir = new DirectoryInfo(OldPath);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(OldPath);
            }

            if (!Directory.Exists(NewPath))
            {
                Directory.CreateDirectory(NewPath);
            }

            foreach (FileInfo file in dir.GetFiles())
            {
                file.CopyTo(Path.Combine(NewPath, file.Name), true);
            }

            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                CopyDirectory(subdir.FullName, Path.Combine(NewPath, subdir.Name));
            }
        }
    }
}