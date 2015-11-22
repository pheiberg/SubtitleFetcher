using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using SubtitleFetcher;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Orchestration;
using SubtitleFetcher.Settings;

namespace UnitTests.SubtitleFetcher
{
    [TestFixture]
    public class DownloaderApplicationTests
    {
        [Test]
        public void Run_FilesAreCleanedUp_CleanedFilesGetsNosrtWritten()
        {
            var fileSystem = A.Fake<IFileOperations>();
            var serializer = A.Fake<IStateSerializer>();
            var subtitleState = new SubtitleState();
            subtitleState.Entries.Add(new SubtitleStateEntry("cleaned1", DateTime.Now.AddDays(-100)));
            subtitleState.Entries.Add(new SubtitleStateEntry("cleaned2", DateTime.Now.AddDays(-101)));
            subtitleState.Entries.Add(new SubtitleStateEntry("stays", DateTime.Now.AddDays(-1)));
            A.CallTo(() => serializer.LoadState()).Returns(subtitleState);
            var fileProcessor = A.Fake<IFileProcessor>();
            var application = new DownloaderApplication(fileSystem, serializer, fileProcessor);
            var options = new Options {GiveupDays = 99};

            application.Run(options);
            
            A.CallTo(() => fileSystem.CreateNosrtFile(A<SubtitleStateEntry>.That.Matches(s => s.File.StartsWith("cleaned")))).MustHaveHappened(Repeated.Exactly.Times(2));
            A.CallTo(() => fileSystem.CreateNosrtFile(A<SubtitleStateEntry>.That.Matches(s => s.File.StartsWith("stays")))).MustNotHaveHappened();
        }

        [Test]
        public void Run_IfFilesFail_AddsThemToState()
        {
            var files = new[] {"fail1.avi", "success.avi", "fail2.avi"};
            var fileSystem = A.Fake<IFileOperations>();
            A.CallTo(() => fileSystem.GetFilesToProcess(A<IEnumerable<string>>._, A<IEnumerable<string>>._)).Returns(files);
            var serializer = A.Fake<IStateSerializer>();
            var subtitleState = new SubtitleState();
            A.CallTo(() => serializer.LoadState()).Returns(subtitleState);
            var fileProcessor = A.Fake<IFileProcessor>();
            A.CallTo(() => fileProcessor.ProcessFile(A<string>.That.StartsWith("fail"), A<IEnumerable<string>>._)).Returns(false);
            A.CallTo(() => fileProcessor.ProcessFile(A<string>.That.StartsWith("success"), A<IEnumerable<string>>._)).Returns(true);
            var application = new DownloaderApplication(fileSystem, serializer, fileProcessor);
            var options = new Options { GiveupDays = 99 };

            application.Run(options);

            Assert.That(subtitleState.Entries.Select(s => s.File), Is.EquivalentTo(new []{ "fail1.avi", "fail2.avi"}));
        }

        [Test]
        public void Run_Always_SavesState()
        {
            var fileSystem = A.Fake<IFileOperations>();
            var serializer = A.Fake<IStateSerializer>();
            var subtitleState = new SubtitleState();
            A.CallTo(() => serializer.LoadState()).Returns(subtitleState);
            var fileProcessor = A.Fake<IFileProcessor>();
            var application = new DownloaderApplication(fileSystem, serializer, fileProcessor);
            var options = new Options();

            application.Run(options);

            A.CallTo(() => serializer.SaveState(subtitleState)).MustHaveHappened();
        }
    }
}