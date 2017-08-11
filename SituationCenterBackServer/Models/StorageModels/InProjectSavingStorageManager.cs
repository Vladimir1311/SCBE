using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IO = System.IO;

namespace SituationCenterBackServer.Models.StorageModels
{
    //public class InProjectSavingStorageManager : IStorageManager
    //{
    //    private readonly static string storageRoot;
    //    private readonly ILogger<InProjectSavingStorageManager> logger;

    //    static InProjectSavingStorageManager()
    //    {
    //        storageRoot = IO.Path.Combine(IO.Directory.GetCurrentDirectory(), "AppData", "ForStorageFolder");
    //    }

    //    public InProjectSavingStorageManager(ILogger<InProjectSavingStorageManager> logger)
    //    {
    //        this.logger = logger;
    //    }

    //    public DirectoryContent GetContentInFolder(string ownerId, string pathToFolder)
    //    {
    //        var wantedPath = GetDirectoryPathIfCorrect(ownerId, pathToFolder);
    //        return GetContent(wantedPath);
    //    }

    //    public DirectoryContent GetPublicContentInFolder(string ownerId, string pathToFolder)
    //    {
    //        var content = GetContentInFolder(ownerId, pathToFolder);
    //        foreach (var file in content.Files)
    //            ConvertToPublic(file);
    //        foreach (var dir in content.Directories)
    //            dir.Path = PublicPath(dir.Path);
    //        return content;
    //    }

    //    public File Save(string userId, string pathToFolder, IFormFile file)
    //    {
    //        var folderPath = GetDirectoryPathIfCorrect(userId, pathToFolder);
    //        var fileName = NameFromPath(file.FileName);
    //        var filePath = IO.Path.Combine(folderPath, fileName);
    //        using (var filestream = IO.File.Create(filePath))
    //        {
    //            file.CopyTo(filestream);
    //        }
    //        return GetFileInfo(filePath);
    //    }

    //    public File SaveDocument(string userId, string pathToFolder, IFormFile file)
    //    {
    //        var folderPath = GetDirectoryPathIfCorrect(userId, pathToFolder);
    //        var fileName = NameFromPath(file.FileName);
    //        var fileFolderPath = IO.Path.Combine(folderPath, fileName);
    //        IO.Directory.CreateDirectory(fileFolderPath);
    //        var filePath = IO.Path.Combine(fileFolderPath, fileName);
    //        using (var filestream = IO.File.Create(filePath))
    //        {
    //            file.CopyTo(filestream);
    //        }
    //        return GetDocumentInfo(filePath.Substring(0, filePath.LastIndexOf("\\")));
    //    }

    //    public IO.Stream GetFileStream(string localPath)
    //    {
    //        var targetPath = IO.Path.Combine(storageRoot, localPath);
    //        if (IO.Directory.Exists(targetPath))
    //        {
    //            var docName = NameFromPath(localPath);
    //            return IO.File.OpenRead(
    //                IO.Path.Combine(targetPath, docName));
    //        }
    //        else
    //            return IO.File.OpenRead(targetPath);
    //    }

    //    public async Task SavePictureAsync(File file, Picture pic, IO.Stream stream)
    //    {
    //        using (var fileStream = IO.File.Create(IO.Path.Combine(storageRoot, file.Path, pic.Name)))
    //        {
    //            pic.State = PictureState.Downloading;
    //            await stream.CopyToAsync(fileStream);
    //            pic.State = PictureState.Ready;
    //        }
    //    }

    //    public IO.Stream GetFileStream(string ownerId, string pathToFile)
    //    {
    //        var userFolder = GetPathToUserFolder(ownerId);
    //        var wantedPath = IO.Path.Combine(userFolder, pathToFile);
    //        if (IO.File.Exists(wantedPath))
    //            return IO.File.OpenRead(wantedPath);
    //        if (IO.Directory.Exists(wantedPath))
    //            return IO.File.OpenRead(IO.Path.Combine(wantedPath, NameFromPath(wantedPath)));
    //        throw new Exception();
    //    }

    //    public File GetPublicFileInfo(string ownerId, string pathToFile)
    //    {
    //        var userFolder = GetPathToUserFolder(ownerId);
    //        var wantedPath = IO.Path.Combine(userFolder, pathToFile);
    //        var file = GetDocumentInfo(wantedPath);
    //        ConvertToPublic(file);
    //        return file;
    //    }

    //    public File GetFileInfo(string ownerId, string pathToFile)
    //    {
    //        var userFolder = GetPathToUserFolder(ownerId);
    //        var wantedPath = IO.Path.Combine(userFolder, pathToFile);
    //        return GetFileInfo(wantedPath);
    //    }

    //    private void ConvertToPublic(File file)
    //    {
    //        file.Path = PublicPath(file.Path);
    //        file.Pictures.RemoveAll(Pic => Pic == null);
    //        foreach (var pic in file.Pictures.NoNull())
    //            pic.Path = PublicPath(pic.Path);
    //    }

    //    private DirectoryContent GetContent(string path)
    //    {
    //        var folders = IO.Directory.GetDirectories(path)
    //            .Select(P => (name: NameFromPath(P), path: P))
    //            .ToLookup(N => N.name.Contains('.'));
    //        var docs = folders[true]
    //                        .Select(P => P.path)
    //                        .Select(GetDocumentInfo)
    //                        .ToList();
    //        var files = docs.Concat(IO.Directory.GetFiles(path)
    //                .Select(GetFileInfo));

    //        return new DirectoryContent
    //        {
    //            Directories = folders[false]
    //                .Select(N => new Directory { Name = N.name, Path = LocalPath(N.path) })
    //                .ToList(),
    //            Files = files.ToList()
    //        };
    //    }

    //    private File GetDocumentInfo(string pathToDoc)
    //    {
    //        var file = GetFileInfo(pathToDoc);
    //        var pictures = IO.Directory.GetFiles(pathToDoc)
    //            .Select(P => new { path = P, name = NameFromPath(P) })
    //            .Select(V => new { path = V.path, name = V.name, regex = Regex.Match(V.name, @"(\d+)\..*") })
    //            .Where(V => V.regex.Success && V.regex.Value == V.name)
    //            .Select(P => new Picture
    //            {
    //                Name = P.name,
    //                Path = LocalPath(P.path),
    //                State = PictureState.Ready,
    //                Number = int.Parse(P.regex.Groups[1].Value)
    //            });
    //        foreach (var picObj in pictures)
    //            file.Pictures.AnySet(picObj.Number, picObj);
    //        return file;
    //    }

    //    private File GetFileInfo(string pathToFile)
    //    {
    //        var localPath = LocalPath(pathToFile);
    //        return new File()
    //        {
    //            Name = NameFromPath(pathToFile),
    //            State = FileReadyState.Unknow,
    //            Path = localPath
    //        };
    //    }

    //    private string LocalPath(string absolutePath)
    //    {
    //        string localPath = absolutePath.Substring(absolutePath.IndexOf("ForStorageFolder"));
    //        localPath = localPath.Substring("ForStorageFolder\\".Length);

    //        return localPath;
    //    }

    //    private string PublicPath(string localPath)
    //    {
    //        return localPath.Substring("676853f6-5229-4832-b03c-c81b9d8e1606".Length + 1);
    //    }

    //    private string NameFromPath(string path)
    //    {
    //        return path.Substring(path.LastIndexOf('\\') + 1);
    //    }

    //    private string GetDirectoryPathIfCorrect(string userId, string path)
    //    {
    //        string pathToUserFolder = GetPathToUserFolder(userId);
    //        string wantedPath = IO.Path.Combine(pathToUserFolder, path);
    //        if (!IO.Directory.Exists(wantedPath))
    //        {
    //            logger.LogWarning($"directory {wantedPath} is not exist");
    //            throw new Exception("This directory not exist");
    //        }
    //        return wantedPath;
    //    }

    //    private string GetPathToUserFolder(string userId)
    //    {
    //        string wantedPath = IO.Path.Combine(storageRoot, userId);
    //        if (!IO.Directory.Exists(wantedPath))
    //            IO.Directory.CreateDirectory(wantedPath);
    //        return wantedPath;
    //    }
    //}
}