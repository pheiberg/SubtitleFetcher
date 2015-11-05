using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Download;
using SubtitleFetcher.Common.Parsing;

namespace UnitTests.SubtitleFetcher
{
    [TestFixture]
    public class EpisodeSubtitleDownloaderTests
    {
        [Test, AutoFakeData]
        public void SearchSubtitle_ThrowsException_ReturnEmptyResultSet(
            TvReleaseIdentity tvReleaseIdentity,
            string[] languages,
            [Frozen]ISubtitleDownloader downloader,
            EpisodeSubtitleDownloader episodeDownloader)
        {
            A.CallTo(() => downloader.SearchSubtitles(A<SearchQuery>._)).Throws<Exception>();
            
            var results = episodeDownloader.SearchSubtitle(tvReleaseIdentity, languages);

            Assert.That(results, Is.Empty);
        }
        
        [Test, AutoFakeData]
        public void SearchSubtitle_MultipleValidSubtitlesFound_OrderedByLanguagePriority(
            TvReleaseIdentity tvReleaseIdentity,
            string id,
            string programName,
            string[] expectedLanguages,
            string missingLanguage,
            [Frozen]IEpisodeParser nameParser,
            [Frozen]ISubtitleDownloader downloader,
            EpisodeSubtitleDownloader episodeDownloader
            )
        {
            var subtitles = expectedLanguages.Select(l => new Subtitle(id, programName, tvReleaseIdentity.ToString(), l));
            A.CallTo(() => downloader.SearchSubtitles(A<SearchQuery>._)).Returns(subtitles);
            A.CallTo(() => nameParser.ParseEpisodeInfo(A<string>._)).Returns(tvReleaseIdentity);
            var languages = new [] { expectedLanguages[0], missingLanguage, expectedLanguages[1], expectedLanguages[2] };

            var results = episodeDownloader.SearchSubtitle(tvReleaseIdentity, languages);

            Assert.That(results.Select(s => s.LanguageCode), Is.EquivalentTo(expectedLanguages));
        }
        
        [Test, AutoFakeData]
        public void SearchSubtitle_NonEquivalentSubtitlesFound_OnlyIncludesEquivalent(
            TvReleaseIdentity tvReleaseIdentity,
            string id,
            string programName,
            string[] supportedLanguages,
            string otherShow,
            [Frozen]IEpisodeParser nameParser,
            [Frozen]ISubtitleDownloader downloader,
            EpisodeSubtitleDownloader episodeDownloader)
        {
            var anyOfTheSupportedLanguages = supportedLanguages.First();
            var subtitles = new List<Subtitle>
                                {
                new Subtitle(id, programName, tvReleaseIdentity.ToString(), anyOfTheSupportedLanguages),
                new Subtitle(id, programName, tvReleaseIdentity.ToString(), anyOfTheSupportedLanguages),
                new Subtitle(id, programName, otherShow, anyOfTheSupportedLanguages)
            };
            A.CallTo(() => downloader.SearchSubtitles(A<SearchQuery>._)).Returns(subtitles);
            
            var results = episodeDownloader.SearchSubtitle(tvReleaseIdentity, supportedLanguages);

            Assert.That(results.Select(s => s.FileName), Has.All.StringStarting(tvReleaseIdentity.ToString()));
        }
        
        [Test, AutoFakeData]
        public void TryDownloadSubtitle_DownloaderThrowsException_ReturnsFalse(
            Subtitle subtitle,
            string fileName,
            [Frozen]ISubtitleDownloader downloader,
            EpisodeSubtitleDownloader sut)
        {
            A.CallTo(() => downloader.SaveSubtitle(A<Subtitle>._)).Throws<Exception>();
         
            bool result = sut.TryDownloadSubtitle(subtitle, fileName);

            Assert.That(result, Is.False);
        }

        [Test, AutoFakeData]
        public void TryDownloadSubtitle_DownloadSuccessful_RenamesFile(
            Subtitle subtitle,
            string resultFile,
            string fileName,
            [Frozen]ISubtitleDownloader downloader,
            [Frozen]IFileSystem fileSystem,
            EpisodeSubtitleDownloader sut)
        {
            var fileInfo = new FileInfo(resultFile);
            A.CallTo(() => downloader.SaveSubtitle(A<Subtitle>._)).Returns(new List<FileInfo> { fileInfo });

            sut.TryDownloadSubtitle(subtitle, fileName);

            A.CallTo(() => fileSystem.RenameSubtitleFile(fileInfo.FullName, fileName + "." + subtitle.LanguageCode + ".srt")).MustHaveHappened();
        }
        
        [Test, AutoFakeData]
        public void TryDownloadSubtitle_DownloadSuccessful_ReturnsTrue(
            Subtitle subtitle,
            string resultFile,
            string fileName,
            [Frozen]ISubtitleDownloader downloader,
            EpisodeSubtitleDownloader sut)
        {
            A.CallTo(() => downloader.SaveSubtitle(A<Subtitle>._)).Returns(new [] { new FileInfo(resultFile) });
            
            var result = sut.TryDownloadSubtitle(subtitle, fileName);

            Assert.That(result, Is.True);
        }

        [Test, AutoFakeData]
        public void CanHandleAtLeastOneOf_NoLanguages_ReturnsTrue(
            EpisodeSubtitleDownloader sut)
        {
            bool result = sut.CanHandleAtLeastOneOf(new string[0]);

            Assert.That(result, Is.True);
        }
        
        [Test, AutoFakeData]
        public void CanHandleAtLeastOneOf_HasOneOfTheLanguages_ReturnsTrue(
            string handledLanguage,
            string unhandledLanguage,
            [Frozen]ISubtitleDownloader downloader,
            EpisodeSubtitleDownloader sut)
        {
            A.CallTo(() => downloader.LanguageLimitations).Returns(new[] {handledLanguage});

            bool result = sut.CanHandleAtLeastOneOf(new[]{ unhandledLanguage, handledLanguage });

            Assert.That(result, Is.True);
        }
        
        [Test, AutoFakeData]
        public void CanHandleAtLeastOneOf_DoesNotHaveLanguage_ReturnsFalse(
            string[] downloaderLanguages,
            string[] otherLanguages,
            [Frozen]ISubtitleDownloader downloader,
            EpisodeSubtitleDownloader sut)
        {
            A.CallTo(() => downloader.LanguageLimitations).Returns(downloaderLanguages); 
            
            bool result = sut.CanHandleAtLeastOneOf(otherLanguages);

            Assert.That(result, Is.False);
        }

        [Test, AutoFakeData]
        public void CanHandleAtLeastOneOf_DoesNotHaveLanguageLimitations_ReturnsTrue(
            string[] anyLanguages,
            [Frozen]ISubtitleDownloader downloader,
            EpisodeSubtitleDownloader sut)
        {
            A.CallTo(() => downloader.LanguageLimitations).Returns(new string[0]);

            bool result = sut.CanHandleAtLeastOneOf(anyLanguages);

            Assert.That(result, Is.True);
        }
    }
}
