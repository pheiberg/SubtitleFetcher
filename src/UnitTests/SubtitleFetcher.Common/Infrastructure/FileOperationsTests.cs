using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher.Common.Infrastructure;

namespace UnitTests.SubtitleFetcher.Common.Infrastructure
{
    [TestFixture]
    public class FileOperationsTests
    {
        [Test, AutoFakeData]
        public void RenameSubtitleFile_FilePathsGiven_FileSystemMoveIsCalled(
            string source, 
            string target,
            FileBase fileBase,
            [Frozen]IFileSystem fileSystem,
            FileOperations sut
            )
        {
            A.CallTo(() => fileSystem.File).Returns(fileBase);

            sut.RenameSubtitleFile(source, target);
                
            A.CallTo(() => fileBase.Move(source, target)).MustHaveHappened();
        }

        [Test, AutoFakeData]
        public void GetFilesToProcess_NoFilePathsGiven_ChecksIfCurrentDirectoryExists(
            IEnumerable<string> languages,
            DirectoryBase directoryBase,
            [Frozen]IFileSystem fileSystem,
            FileOperations sut
            )
        {
            A.CallTo(() => fileSystem.Directory).Returns(directoryBase);

            var results = sut.GetFilesToProcess(Enumerable.Empty<string>(), languages);

            A.CallTo(() => directoryBase.Exists(".")).MustHaveHappened();
        }
    }
}
