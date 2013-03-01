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
            var processor = new FileProcessor(episodeParser, logger, null, new LanguageSettings(new string[0]));

            bool result = processor.ProcessFile("Unparsable.avi", new string[0]);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ProcessFile_ShowIsIgnored_ReturnsTrue()
        {
            IEpisodeParser episodeParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var processor = new FileProcessor(episodeParser, logger, null, new LanguageSettings(new string[0]));

            bool result = processor.ProcessFile("ignored S01E01-group.avi", new []{"ignored"});

            Assert.That(result, Is.True);
        }

        [Test]
        public void ProcessFile_DownloadsSuccesfully_ReturnsTrue()
        {
            IEpisodeParser episodeParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var subtitleService = A.Fake<ISubtitleDownloadService>();
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, new EpisodeIdentity("show", 1, 1, "group"), A<string[]>._)).Returns(true);
            var processor = new FileProcessor(episodeParser, logger, subtitleService, new LanguageSettings(new string[0]));

            bool result = processor.ProcessFile("show S01E01-group.avi", new string[0]);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ProcessFile_NotDownloaded_ReturnsFalse()
        {
            IEpisodeParser episodeParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var subtitleService = A.Fake<ISubtitleDownloadService>();
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, new string[0])).Returns(false);
            var processor = new FileProcessor(episodeParser, logger, subtitleService, new LanguageSettings(new string[0]));

            bool result = processor.ProcessFile("show S01E01-group.avi", new string[0]);

            Assert.That(result, Is.False);
        }
    }
}