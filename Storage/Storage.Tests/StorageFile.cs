using Moq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Storage.Tests
{
    public class StorageFile : StorageDefaultMoq
    {
        [Fact]
        public void Create()
        {
            var (storage, fs, _, _) = CreateDefault();

            var fileName = "dir1\\dir2\\file.txt";
            var fileContent = "TestString";

            var input = Encoding.ASCII.GetBytes(fileContent);
            var result = new MemoryStream(new byte[input.Length]);

            var fs_createFileName = "";

            fs.Setup(x => x.CreateFile(It.IsAny<string>()))
              .Callback<string>(P => fs_createFileName = P)
              .Returns(result);

            var inputMS = new MemoryStream(input);
            storage.CreateFile("token", "ow", fileName, inputMS);

            Assert.True(fs_createFileName.EndsWith(fileName));

            Assert.Equal(inputMS.Length, inputMS.Position);
            Assert.False(result.CanRead);
        }

        [Fact]
        public void Delete()
        {
            var (storage, fs, _, _) = CreateDefault();

            var fileName = "dir1\\dir2\\file.txt";

            var fs_DeleteFileName = "";

            fs.Setup(x => x.DeleteFile(It.IsAny<string>()))
              .Callback<string>(P => fs_DeleteFileName = P);

            fs.Setup(x => x.FileExists(It.IsAny<string>()))
              .Returns(true);

            Assert.True(storage.DeleteFile("token", "ow", fileName));
            Assert.True(fs_DeleteFileName.EndsWith(fileName));
        }

        [Fact]
        public void Exist()
        {
            var (storage, fs, _, _) = CreateDefault();

            var variants = new Dictionary<string, bool>()
            {
                { @"file.txt", true },
                { @"dir1\dir2\file.txt", true },
                { @"dir1\dir2", false },
                { @"dir1\dir2\file2.txt", false },
                { @"dir1\dir2\file.txt\page2", false },
            };

            fs.Setup(x => x.FileExists(It.IsAny<string>()))
              .Returns<string>(x => variants.GetValueOrDefault(x, false));

            foreach (var variant in variants)
            {
                Assert.Equal(storage.IsExistFile("token", "ow", variant.Key), variant.Value);
            }
        }

        [Fact]
        public void Move()
        {
            var (storage, fs, _, _) = CreateDefault();

            var fileName = "dir1\\dir2\\file.txt";
            var fileNameNew = "dir1\\new\\dudu.txt";

            var fs_MoveFileNameOld = "";
            var fs_MoveFileNameNew = "";

            fs.Setup(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()))
              .Callback<string, string>(
                (old, n) =>
                {
                    fs_MoveFileNameOld = old;
                    fs_MoveFileNameNew = n;
                });

            fs.Setup(x => x.FileExists(It.IsAny<string>()))
              .Returns(true);

            Assert.True(storage.Move("token", "ow", fileName, fileNameNew));
            Assert.True(fs_MoveFileNameOld.EndsWith(fileName));
            Assert.True(fs_MoveFileNameNew.EndsWith(fileNameNew));
        }

        [Fact]
        public void IsUseAccess()
        {
            var (storage, fs, _, _) = CreateDefault();

            fs.Setup(x => x.FileExists(It.IsAny<string>()))
              .Returns(true);

            Assert.True(storage.IsExistFile("token", "ow", "dir1\\dir2\\file.txt"));
            Assert.False(storage.IsExistFile("bad_token", "ow", "dir1\\dir2\\file.txt"));
        }
    }
}
