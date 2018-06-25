using Moq;
using System.IO;
using System.Text;
using Xunit;

namespace Storage.Tests
{
    public class StorageDocuments : StorageDefaultMoq
    {
        [Fact]
        public void Create()
        {
            var (storage, fs, pm, _) = CreateDefault();

            var fileName = "dir1\\dir2\\file.doc";
            var fileContent = "TestString";

            var input = Encoding.ASCII.GetBytes(fileContent);
            var result = new MemoryStream(new byte[input.Length]);

            var fs_createFileName = "";
            var pm_processFileName = "";

            fs.Setup(x => x.CreateFile(It.IsAny<string>()))
              .Callback<string>(P => fs_createFileName = P)
              .Returns(result);

            pm.Setup(x => x.ProcessDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>(
                (_, path) => pm_processFileName = path);

            var inputMS = new MemoryStream(input);
            storage.CreateFile("token", "ow", fileName, inputMS);

            Assert.True(fs_createFileName.EndsWith(fileName));
            Assert.True(pm_processFileName.EndsWith(fileName));

            Assert.Equal(inputMS.Length, inputMS.Position);
            Assert.False(result.CanRead);
        }
    }
}
