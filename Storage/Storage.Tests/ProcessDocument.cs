using DocsToPictures.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using Storage.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Xunit;
using System.Threading;
using System;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Storage.Tests
{
    public class ProcessDocument
    {
        [Fact]
        public void UseMeta()
        {
            var (pm, fs, dp) = CreateDefault();

            var PagesCount = 0;
            var GetAvailablePages = new List<int>();
            var doc_stream = new MemoryStream(Encoding.ASCII.GetBytes("simple"));

            var fs_createDir = "";
            var fs_writeFileName = "";
            Stream dp_stream = null;

            var doc = new Mock<IDocument>();
            doc.Setup(x => x.PagesCount).Returns(PagesCount);
            doc.Setup(x => x.GetAvailablePages()).Returns(GetAvailablePages);

            dp.Setup(x => x.AddToHandle(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback<string, Stream>((_, stream) => dp_stream = stream)
                .Returns(doc.Object);

            fs.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(false);
            fs.Setup(x => x.ReadInFile(It.IsAny<string>())).Returns(doc_stream);
            fs.Setup(x => x.CreateDirectory(It.IsAny<string>())).Callback<string>(name => fs_createDir = name);

            fs.Setup(x => x.WriteAllTextToFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((name, _) => fs_writeFileName = name);

            var fileName = "dir1\\dir2\\file.doc";

            pm.ProcessDocument("ow", fileName);
            Thread.Sleep(150);

            Assert.True(fs_createDir.EndsWith(fileName));
            Assert.True(fs_writeFileName.EndsWith(StorageDefaultMoq.StorageSetting.DocumentMetaFileName));
            Assert.Equal(doc_stream, dp_stream);
        }

        [Fact]
        public void ProcessPages()
        {
            var (pm, fs, dp) = CreateDefault();

            var PagesCount = 10;
            var GetAvailablePages = Enumerable.Range(1, PagesCount).ToList();
            var doc_stream = new MemoryStream(Encoding.ASCII.GetBytes("simple"));
            var doc_wireCount = 0;
            var rand = new Random(DateTime.Now.Millisecond);

            var pic_streams = new List<MemoryStream>();
            pic_streams.Add(null);

            for (int i = 1; i <= PagesCount; i++)
            {
                pic_streams.Add(new MemoryStream(Encoding.ASCII.GetBytes("jpeeeg_" + i)));
            }

            var fs_writeFileName = "";
            Stream dp_stream = null;

            var doc = new Mock<IDocument>();
            doc.Setup(x => x.PagesCount).Returns(PagesCount);
            doc.Setup(x => x.GetPicture(It.IsAny<int>())).Returns<int>(i => pic_streams[i]);
            doc.Setup(x => x.GetAvailablePages())
                .Returns(() =>
                {
                    var st = rand.Next(1, GetAvailablePages.Count);
                    var ct = rand.Next(1, GetAvailablePages.Count - st + 1);
                    var range = GetAvailablePages.GetRange(st, ct).ToList();
                    GetAvailablePages.RemoveAll(i => range.Contains(i));
                    return range;
                });

            dp.Setup(x => x.AddToHandle(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback<string, Stream>((_, stream) => dp_stream = stream)
                .Returns(doc.Object);

            fs.Setup(x => x.DirectoryExists(It.IsAny<string>())).Returns(true);
            fs.Setup(x => x.ReadInFile(It.IsAny<string>())).Returns(doc_stream);
            fs.Setup(x => x.WriteToFile(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback<string, Stream>((_, __) => doc_wireCount++);

            fs.Setup(x => x.WriteAllTextToFile(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((name, _) => fs_writeFileName = name);

            var fileName = "dir1\\dir2\\file.doc";

            pm.ProcessDocument("ow", fileName);
            Thread.Sleep(1000);

            Assert.True(fs_writeFileName.EndsWith(StorageDefaultMoq.StorageSetting.DocumentMetaFileName));
            Assert.Equal(doc_stream, dp_stream);
            Assert.Equal(PagesCount, doc_wireCount);
        }

        public (IDocumentPageManager, Mock<IFileSystem>, Mock<IDocumentProcessor>) CreateDefault()
        {
            var fs = new Mock<IFileSystem>();
            fs.Setup(x => x.GetFileExtension(It.IsAny<string>())).Returns((string x) => Path.GetExtension(x));
            fs.Setup(x => x.GetFileName(It.IsAny<string>())).Returns((string x) => Path.GetFileName(x));
            fs.Setup(x => x.CombinePath(It.IsAny<string[]>())).Returns((string[] x) => Path.Combine(x));

            var opt1 = new Mock<IOptions<StorageSetting>>();
            opt1.SetupGet(a => a.Value).Returns(StorageDefaultMoq.StorageSetting);

            var opt2 = new Mock<IOptions<PageManagerSetting>>();
            opt2.SetupGet(a => a.Value).Returns(StorageDefaultMoq.PageManagerSetting);

            var dp = new Mock<IDocumentProcessor>();

            var logger = new Mock<ILogger<DocumentPageManager>>();
            
            var serviceProvider = new Mock<IServiceProvider>();

            var documentPageManager = new DocumentPageManager(logger.Object, opt1.Object, opt2.Object, serviceProvider.Object, fs.Object, dp.Object);

            return (documentPageManager, fs, dp);
        }
    }
}
