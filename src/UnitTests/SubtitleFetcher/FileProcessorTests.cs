using FakeItEasy;
using NUnit.Framework;
using SubtitleFetcher;
using SubtitleFetcher.Common;

namespace UnitTests.SubtitleFetcher
{
    [TestFixture]
    public class FileProcessorTests
    {
        [Test]
        public void ProcessFile_CannotParseName_ReturnsTrue()
        {
            var episodeParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var processor = new FileProcessor(episodeParser, logger, null, null, null);

            bool result = processor.ProcessFile("Unparsable.avi", new string[0]);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ProcessFile_ShowIsIgnored_ReturnsTrue()
        {
            IEpisodeParser episodeParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var processor = new FileProcessor(episodeParser, logger, null, null, null);

            bool result = processor.ProcessFile("ignored S01E01-group.avi", new[] { "ignored" });

            Assert.That(result, Is.True);
        }

        [Test]
        public void ProcessFile_DownloadsSuccesfully_ReturnsTrue()
        {
            IEpisodeParser episodeParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var subtitleService = A.Fake<ISubtitleDownloadService>();
            var fileSystem = A.Fake<IFileSystem>();
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, new EpisodeIdentity("show", 1, 1, "group"), A<string[]>._)).Returns(true);
            var processor = new FileProcessor(episodeParser, logger, subtitleService, fileSystem, new LanguageSettings(new string[0]));

            bool result = processor.ProcessFile("show S01E01-group.avi", new string[0]);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ProcessFile_NotDownloaded_ReturnsFalse()
        {
            IEpisodeParser episodeParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var fileSystem = A.Fake<IFileSystem>();
            var subtitleService = A.Fake<ISubtitleDownloadService>();
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, A<string[]>._)).Returns(false);
            var processor = new FileProcessor(episodeParser, logger, subtitleService, fileSystem, new LanguageSettings(new string[0]));

            bool result = processor.ProcessFile("show S01E01-group.avi", new string[0]);

            Assert.That(result, Is.False);
        }

        [Test]
        public void ProcessFile_SomeLanguagesAlreadyDownloaded_TriesToDownloadOnlyNotDownloaded()
        {
            var languagesToFetch = new[] { "swe", "dan", "nor", "eng" };
            var downloadedLanguages = new[] { "dan", "eng" };
            IEpisodeParser episodeParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var subtitleService = A.Fake<ISubtitleDownloadService>();
            var fileSystem = A.Fake<IFileSystem>();
            A.CallTo(() => fileSystem.GetDowloadedSubtitleLanguages(A<string>._, languagesToFetch)).Returns(downloadedLanguages);
            var processor = new FileProcessor(episodeParser, logger, subtitleService, fileSystem, new LanguageSettings(languagesToFetch));

            bool result = processor.ProcessFile("show S01E01-group.avi", new string[0]);

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, A<string[]>.That.IsSameSequenceAs(new[] { "swe", "nor" }))).MustHaveHappened();
        }
    }
}