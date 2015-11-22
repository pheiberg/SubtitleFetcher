using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Orchestration;
using SubtitleFetcher.Common.Parsing;

namespace UnitTests.SubtitleFetcher.Common.Orchestration
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

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<Language>>._)).MustNotHaveHappened();
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

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<Language>>._)).MustNotHaveHappened();
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
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<Language>>._))
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
            A.CallTo(() => fileSystem.GetDowloadedSubtitleLanguages(fileName, 
                A<IEnumerable<Language>>.That.IsSameSequenceAs(languages)))
               .Returns(alreadyDownloadedLanguages);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(tvRelease);

            sut.ProcessFile(fileName, new string[0]);

            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<Language[]>
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
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<Language>>._)).Returns(false);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(tvRelease);
            
            var result = sut.ProcessFile(fileName, ignored);

            Assert.That(result, Is.False);
        }

        [Test, AutoFakeData]
        public void ProcessFile_AllLanguagesAlreadyDownloaded_ReturnsTrue( 
            Language[] languages,
            string fileName,
            TvReleaseIdentity tvRelease,
            ILogger logger,
            IEpisodeParser episodeParser,
            ISubtitleDownloadService subtitleService,
            [Frozen]IFileSystem fileSystem,
            FileProcessor sut)
        {
            var settings = new LanguageSettings(languages);
            A.CallTo(() => fileSystem.GetDowloadedSubtitleLanguages(A<string>._, languages)).Returns(languages);
            A.CallTo(() => subtitleService.DownloadSubtitle(A<string>._, A<TvReleaseIdentity>._, A<IEnumerable<Language>>._))
                .Returns(false);
            A.CallTo(() => episodeParser.ParseEpisodeInfo(A<string>._)).Returns(tvRelease);
            var processor = new FileProcessor(episodeParser, logger, subtitleService, fileSystem, settings);

            bool result = processor.ProcessFile(fileName, new string[0]);

            Assert.That(result, Is.True);
        }
    }
}