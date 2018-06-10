using Microsoft.Extensions.Options;
using Moq;
using SituationCenterBackServer.Interfaces;
using Storage.Interfaces;
using System.IO;

namespace Storage.Tests
{
    public class StorageDefaultMoq
    {
        public static StorageSetting StorageSetting = new StorageSetting
        {
            PathToUserSpaces = "AppData/Users",
            PathToDocumentSpaces = "AppData/Doc",
            DocumentMetaFileName = "meta.json",
            DocumentMetaPageExtension = ".png",
            DocumentPaintExtension = ".vec"
        };

        public static PageManagerSetting PageManagerSetting = new PageManagerSetting
        {
            StartMaxTryCount = 3,
            StartTryDelay = 5,
            GetPageMaxTryCount = 3,
            GetPageTryDelay = 5,
        };

        public (IStorage, Mock<IFileSystem>, Mock<IDocumentPageManager>, Mock<IAccessValidator>) CreateDefault()
        {
            var accessValidator = new Mock<IAccessValidator>();

            accessValidator
                .Setup(a => a.CanAccessToFolder(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((t, _) => t != "bad_token");

            var opt = new Mock<IOptions<StorageSetting>>();
            opt.SetupGet(a => a.Value).Returns(StorageSetting);

            var fs = new Mock<IFileSystem>();
            fs.Setup(x => x.GetFileExtension(It.IsAny<string>())).Returns((string x) => Path.GetExtension(x));
            fs.Setup(x => x.GetFileName(It.IsAny<string>())).Returns((string x) => Path.GetFileName(x));
            fs.Setup(x => x.CombinePath(It.IsAny<string[]>())).Returns((string[] x) => Path.Combine(x));

            var documentPageManager = new Mock<IDocumentPageManager>();

            var storage = new DocumentSupportStorage(opt.Object, documentPageManager.Object, accessValidator.Object, fs.Object);

            return (storage, fs, documentPageManager, accessValidator);
        }
    }
}
