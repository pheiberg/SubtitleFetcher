using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher;
using SubtitleFetcher.Common;

namespace UnitTests.SubtitleFetcher
{
    [TestFixture]
    public class FileProcessorTests
    {
        [Test, AutoFakeData]
        public void ProcessFile_CannotParseName_ReturnsTrue(
            string fileName,
            string[] ignored,
            EpisodeIdentity episode,
            [Frozen] IEpisodeParser episodeParser,
            FileProcessor sut)
        {
            var emptyEpisode = new EpisodeIdentity();
            A.CallTo(() => episodeParser.ParseEpisodeInfo(fileName)).Returns(emptyEpisode);

            bool result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.True);
        }

        [Test, AutoFakeData]
        public void ProcessFile_CannotParseName_IsNotDownloaded(
            string fileName,
            string[] ignored,
            EpisodeIdentity episode,
            [Frozen] IEpisodeParser episodeParser,
            [Frozen] ISubtitleDownloadService subtitleService,
            FileProcessor sut)
        {
            var emptyEpisode = new EpisodeIdentity();
            A.CallTo(() => episodeParser.ParseEpisodeInfo(fileName)).Returns(emptyEpisode);

            sut.ProcessFile(fileName, ignored);

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, A<IEnumerable<string>>._)).MustNotHaveHappened();
        }

        [Test, AutoFakeData]
        public void ProcessFile_ShowIsIgnored_ReturnsTrue(
            string fileName,
            [Frozen]string[] ignored,
            EpisodeIdentity episode,
            [Frozen] IEpisodeParser episodeParser,
            FileProcessor sut)
        {
            episode.SeriesName = ignored.First();
            A.CallTo(() => episodeParser.ParseEpisodeInfo(fileName)).Returns(episode);

            bool result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.True);
        }

        [Test, AutoFakeData]
        public void ProcessFile_ShowIsIgnored_DoesNotDownload(
            string fileName,
            string[] ignored,
            EpisodeIdentity episode,
            [Frozen] IEpisodeParser episodeParser,
            [Frozen] ISubtitleDownloadService subtitleService,
            FileProcessor sut)
        {
            episode.SeriesName = ignored.First();
            A.CallTo(() => episodeParser.ParseEpisodeInfo(fileName)).Returns(episode);

            sut.ProcessFile(fileName, ignored);

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, A<IEnumerable<string>>._)).MustNotHaveHappened();
        }

        [Test, AutoFakeData]
        public void ProcessFile_DownloadsSuccesfully_ReturnsTrue(
            string fileName,
            string[] ignored,
            EpisodeIdentity episode,
            [Frozen] IEpisodeParser episodeParser,
            [Frozen] ISubtitleDownloadService subtitleService,
            FileProcessor sut)
        {
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, A<IEnumerable<string>>._))
                .Returns(true);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(episode);

            var result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.True);
        }

        [Test, AutoFakeData]
        public void ProcessFile_NotDownloaded_ReturnsFalse(
            string fileName,
            string[] ignored,            
            EpisodeIdentity episode,
            [Frozen]IEpisodeParser episodeParser,
            [Frozen]ISubtitleDownloadService subtitleService,
            FileProcessor sut)
        {
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, A<IEnumerable<string>>._)).Returns(false);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(episode);
            
            var result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.False);
        }

        [Test, AutoFakeData]
        public void ProcessFile_SomeLanguagesAlreadyDownloaded_TriesToDownloadOnlyNotDownloaded(
            [Frozen]string[] languages,
            string fileName,
            EpisodeIdentity episode,
            [Frozen]IEpisodeParser episodeParser,
            [Frozen]ISubtitleDownloadService subtitleService,
            [Frozen]IFileSystem fileSystem,
            FileProcessor sut)
        {
            var alreadyDownloadedLanguages = languages.Skip(1);
            var expected = languages.Take(1);
            A.CallTo(() => fileSystem.GetDowloadedSubtitleLanguages(A<string>._, languages))
                .Returns(alreadyDownloadedLanguages);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(episode);

            sut.ProcessFile(fileName, new string[0]);

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, A<string[]>.That.IsSameSequenceAs(expected))).MustHaveHappened();
        }

        [Test, AutoFakeData]
        public void ProcessFile_AllLanguagesAlreadyDownloaded_ReturnsTrue(
            string[] languages,
            string fileName,
            EpisodeIdentity episode,
            ILogger logger,
            IEpisodeParser episodeParser,
            ISubtitleDownloadService subtitleService,
            [Frozen]IFileSystem fileSystem,
            FileProcessor sut)
        {
            var settings = new LanguageSettings(languages);
            A.CallTo(() => fileSystem.GetDowloadedSubtitleLanguages(A<string>._, languages)).Returns(languages);
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<EpisodeIdentity>._, A<IEnumerable<string>>._))
                .Returns(false);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(episode);
            var processor = new FileProcessor(episodeParser, logger, subtitleService, fileSystem, settings);

            bool result = processor.ProcessFile(fileName, new string[0]);

            Assert.That(result, Is.True);
        }
    }
}