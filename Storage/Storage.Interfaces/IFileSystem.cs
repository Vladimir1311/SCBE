using System.Collections.Generic;
using System.IO;

namespace Storage.Interfaces
{
    public interface IFileSystem
    {
        bool DirectoryExists(string Path);

        bool FileExists(string Path);

        IEnumerable<string> GetDirectories(string Path);

        IEnumerable<string> GetFiles(string Path);

        long GetFileSize(string Path);

        Stream CreateFile(string Path);

        Stream ReadInFile(string Path);

        string CombinePath(params string[] paths);

        string GetFileExtension(string Path);

        string GetFileName(string Path);

        string NormalizePath(string Path);

        string ReadAllTextInFile(string Path);

        void CopyDirectory(string OldPath, string NewPath);

        void CopyFile(string OldPath, string NewPath);

        void CreateDirectory(string Path);

        void DeleteDirectory(string Path, bool Recursive);

        void DeleteFile(string Path);

        void MoveDirectory(string OldPath, string NewPath);

        void MoveFile(string OldPath, string NewPath);

        void WriteAllTextToFile(string Path, string Content);

        void WriteToFile(string Path, Stream Content);
    }
}