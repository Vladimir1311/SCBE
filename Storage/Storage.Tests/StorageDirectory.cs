using Moq;
using System.Collections.Generic;
using Xunit;

namespace Storage.Tests
{
    public class StorageDirectory : StorageDefaultMoq
    {
        [Fact]
        public void Create()
        {
            var (storage, fs, _, _) = CreateDefault();

            var dirName = "dir1\\dir2";

            var fs_createdirName = "";

            fs.Setup(x => x.CreateDirectory(It.IsAny<string>()))
              .Callback<string>(P => fs_createdirName = P);

            storage.CreateDirectory("token", "ow", dirName);

            Assert.True(fs_createdirName.EndsWith(dirName));
        }

        [Fact]
        public void Delete()
        {
            var (storage, fs, _, _) = CreateDefault();

            var dirName = "dir1\\dir2";

            var fs_DeletedirName = "";

            fs.Setup(x => x.DeleteDirectory(It.IsAny<string>(), It.IsAny<bool>()))
              .Callback<string, bool>((P, _) => fs_DeletedirName = P);

            fs.Setup(x => x.DirectoryExists(It.IsAny<string>()))
              .Returns(true);

            Assert.True(storage.DeleteDirectory("token", "ow", dirName));
            Assert.True(fs_DeletedirName.EndsWith(dirName));
        }

        [Fact]
        public void Exist()
        {
            var (storage, fs, _, _) = CreateDefault();

            var variants = new Dictionary<string, bool>()
            {
                { @"file.txt", false },
                { @"dir1\dir2\file.txt", false },
                { @"dir1\dir2", true },
                { @"dir1\dir2\dir", true },
                { @"dir1\dir2\dir.3\dir4", true },
            };

            fs.Setup(x => x.DirectoryExists(It.IsAny<string>()))
              .Returns<string>(x => variants.GetValueOrDefault(x, false));

            foreach (var variant in variants)
            {
                Assert.Equal(storage.IsExistDirectory("token", "ow", variant.Key), variant.Value);
            }
        }

        [Fact]
        public void Move()
        {
            var (storage, fs, _, _) = CreateDefault();

            var dirName = "dir1\\dir2";
            var dirNameNew = "dir1\\dir3";

            var fs_MovedirNameOld = "";
            var fs_MovedirNameNew = "";

            fs.Setup(x => x.MoveFile(It.IsAny<string>(), It.IsAny<string>()))
              .Callback<string, string>(
                (old, n) =>
                {
                    fs_MovedirNameOld = old;
                    fs_MovedirNameNew = n;
                });

            fs.Setup(x => x.DirectoryExists(It.IsAny<string>()))
              .Returns(true);

            Assert.True(storage.Move("token", "ow", dirName, dirNameNew));
            Assert.True(fs_MovedirNameOld.EndsWith(dirName));
            Assert.True(fs_MovedirNameNew.EndsWith(dirNameNew));
        }

        [Fact]
        public void IsUseAccess()
        {
            var (storage, fs, _, _) = CreateDefault();

            fs.Setup(x => x.FileExists(It.IsAny<string>()))
              .Returns(true);

            Assert.True(storage.IsExistFile("token", "ow", "dir1\\dir2"));
            Assert.False(storage.IsExistFile("bad_token", "ow", "dir1\\dir2"));
        }
    }
}
