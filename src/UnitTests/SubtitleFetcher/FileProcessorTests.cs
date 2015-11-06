using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders.SubDb;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Parsing;

namespace UnitTests.SubtitleFetcher
{
    [TestFixture]
    public class FileProcessorTests
    {
        [Test, AutoFakeData]
        public void ProcessFile_CannotParseName_ReturnsTrue(
            string fileName,
            string[] ignored,
            TvReleaseIdentity tvRelease,
            [Frozen] IEpisodeParser episodeParser,
            FileProcessor sut)
        {
            var emptyEpisode = new TvReleaseIdentity();
            A.CallTo(() => episodeParser.ParseEpisodeInfo(fileName)).Returns(emptyEpisode);

            bool result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.True);
        }

        [Test, AutoFakeData]
        public void ProcessFile_CannotParseName_IsNotDownloaded(
            string fileName,
            string[] ignored,
            TvReleaseIdentity tvRelease,
            [Frozen] IEpisodeParser episodeParser,
            [Frozen] ISubtitleDownloadService subtitleService,
            FileProcessor sut)
        {
            var emptyEpisode = new TvReleaseIdentity();
            A.CallTo(() => episodeParser.ParseEpisodeInfo(fileName)).Returns(emptyEpisode);

            sut.ProcessFile(fileName, ignored);

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<string>>._)).MustNotHaveHappened();
        }

        [Test, AutoFakeData]
        public void ProcessFile_ShowIsIgnored_ReturnsTrue(
            string fileName,
            [Frozen]string[] ignored,
            TvReleaseIdentity tvRelease,
            [Frozen] IEpisodeParser episodeParser,
            FileProcessor sut)
        {
            tvRelease.SeriesName = ignored.First();
            A.CallTo(() => episodeParser.ParseEpisodeInfo(fileName)).Returns(tvRelease);

            bool result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.True);
        }

        [Test, AutoFakeData]
        public void ProcessFile_ShowIsIgnored_DoesNotDownload(
            string fileName,
            string[] ignored,
            TvReleaseIdentity tvRelease,
            [Frozen] IEpisodeParser episodeParser,
            [Frozen] ISubtitleDownloadService subtitleService,
            FileProcessor sut)
        {
            tvRelease.SeriesName = ignored.First();
            A.CallTo(() => episodeParser.ParseEpisodeInfo(fileName)).Returns(tvRelease);

            sut.ProcessFile(fileName, ignored);

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<string>>._)).MustNotHaveHappened();
        }

        [Test, AutoFakeData]
        public void ProcessFile_DownloadsSuccesfully_ReturnsTrue(
            string fileName,
            string[] ignored,
            TvReleaseIdentity tvRelease,
            [Frozen] IEpisodeParser episodeParser,
            [Frozen] ISubtitleDownloadService subtitleService,
            FileProcessor sut)
        {
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<string>>._))
                .Returns(true);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(tvRelease);

            var result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.True);
        }

        [Test, AutoFakeData]
        public void ProcessFile_SomeLanguagesAlreadyDownloaded_TriesToDownloadOnlyNotDownloaded(
            string fileName,
            TvReleaseIdentity tvRelease,
            [Frozen]LanguageSettings languageSettings,
            [Frozen]IEpisodeParser episodeParser,
            [Frozen]ISubtitleDownloadService subtitleService,
            [Frozen]IFileSystem fileSystem,
            FileProcessor sut)
        {
            var languages = languageSettings.Languages.ToArray();
            var alreadyDownloadedLanguages = languages.Skip(1);
            var expected = languages.Take(1);
            A.CallTo(() => fileSystem.GetDowloadedSubtitleLanguages(A<string>._, 
                A<IEnumerable<string>>.That.IsSameSequenceAs(languages)))
               .Returns(alreadyDownloadedLanguages);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(tvRelease);

            sut.ProcessFile(fileName, new string[0]);

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<string[]>
                .That.IsSameSequenceAs(expected))).MustHaveHappened();
        }

        [Test, AutoFakeData]
        public void ProcessFile_NotDownloaded_ReturnsFalse(
            string fileName,
            string[] ignored,            
            TvReleaseIdentity tvRelease,
            [Frozen]IEpisodeParser episodeParser,
            [Frozen]ISubtitleDownloadService subtitleService,
            FileProcessor sut)
        {
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<string>>._)).Returns(false);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(tvRelease);
            
            var result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.False);
        }

        [Test, AutoFakeData]
        public void ProcessFile_AllLanguagesAlreadyDownloaded_ReturnsTrue( 
            string[] languages,
            string fileName,
            TvReleaseIdentity tvRelease,
            ILogger logger,
            IEpisodeParser episodeParser,
            ISubtitleDownloadService subtitleService,
            ISubDbHasher hasher,
            [Frozen]IFileSystem fileSystem,
            FileProcessor sut)
        {
            var settings = new LanguageSettings(languages);
            A.CallTo(() => fileSystem.GetDowloadedSubtitleLanguages(A<string>._, languages)).Returns(languages);
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<string>>._))
                .Returns(false);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(tvRelease);
            var processor = new FileProcessor(episodeParser, logger, subtitleService, fileSystem, settings, hasher);

            bool result = processor.ProcessFile(fileName, new string[0]);

            Assert.That(result, Is.True);
        }
    }
}