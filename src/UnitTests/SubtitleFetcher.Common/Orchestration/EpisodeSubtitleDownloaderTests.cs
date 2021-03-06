﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Orchestration;
using SubtitleFetcher.Common.Parsing;

namespace UnitTests.SubtitleFetcher.Common.Orchestration
{
    [TestFixture]
    public class EpisodeSubtitleDownloaderTests
    {
        [Test, AutoFakeData]
        public void SearchSubtitle_ThrowsException_ReturnEmptyResultSet(
            TvReleaseIdentity tvReleaseIdentity,
            Language[] languages,
            [Frozen]ISubtitleDownloader downloader,
            SubtitleDownloaderWrapper downloaderWrapper)
        {
            A.CallTo(() => downloader.SearchSubtitles(A<SearchQuery>._)).Throws<Exception>();
            
            var results = downloaderWrapper.SearchSubtitle(tvReleaseIdentity, languages);

            Assert.That(results, Is.Empty);
        }
        
        [Test, AutoFakeData]
        public void SearchSubtitle_MultipleValidSubtitlesFound_OrderedByLanguagePriority(
            TvReleaseIdentity tvReleaseIdentity,
            string id,
            string fileName,
            Language[] expectedLanguages,
            Language missingLanguage,
            [Frozen]IEpisodeParser nameParser,
            [Frozen]ISubtitleDownloader downloader,
            SubtitleDownloaderWrapper downloaderWrapper
            )
        {
            var subtitles = expectedLanguages.Select(l => new Subtitle(id, fileName, l));
            A.CallTo(() => downloader.SearchSubtitles(A<SearchQuery>._)).Returns(subtitles);
            A.CallTo(() => nameParser.ExtractReleaseIdentity(A<Subtitle>._)).Returns(tvReleaseIdentity);
            var languages = new [] { expectedLanguages[0], missingLanguage, expectedLanguages[1], expectedLanguages[2] };

            var results = downloaderWrapper.SearchSubtitle(tvReleaseIdentity, languages);

            Assert.That(results.Select(s => s.Language), Is.EquivalentTo(expectedLanguages));
        }
        
        [Test, AutoFakeData]
        public void SearchSubtitle_NonEquivalentSubtitlesFound_OnlyIncludesEquivalent(
            TvReleaseIdentity tvReleaseIdentity,
            string id,
            string programName,
            Language[] supportedLanguages,
            string otherShow,
            [Frozen]IEpisodeParser nameParser,
            [Frozen]ISubtitleDownloader downloader,
            SubtitleDownloaderWrapper downloaderWrapper)
        {
            var anyOfTheSupportedLanguages = supportedLanguages.First();
            var subtitles = new List<Subtitle>
                                {
                new Subtitle(id, tvReleaseIdentity.ToString(), anyOfTheSupportedLanguages),
                new Subtitle(id, tvReleaseIdentity.ToString(), anyOfTheSupportedLanguages),
                new Subtitle(id, otherShow, anyOfTheSupportedLanguages)
            };
            A.CallTo(() => downloader.SearchSubtitles(A<SearchQuery>._)).Returns(subtitles);
            
            var results = downloaderWrapper.SearchSubtitle(tvReleaseIdentity, supportedLanguages);

            Assert.That(results.Select(s => s.FileName), Has.All.StringStarting(tvReleaseIdentity.ToString()));
        }
        
        [Test, AutoFakeData]
        public void TryDownloadSubtitle_DownloaderThrowsException_ReturnsFalse(
            Subtitle subtitle,
            string fileName,
            [Frozen]ISubtitleDownloader downloader,
            SubtitleDownloaderWrapper sut)
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
            [Frozen]IFileOperations fileOperations,
            SubtitleDownloaderWrapper sut)
        {
            var fileInfo = new FileInfo(resultFile);
            A.CallTo(() => downloader.SaveSubtitle(A<Subtitle>._)).Returns(new List<FileInfo> { fileInfo });

            sut.TryDownloadSubtitle(subtitle, fileName);

            A.CallTo(() => fileOperations.RenameSubtitleFile(fileInfo.FullName, fileName + "." + subtitle.Language.TwoLetterIsoName + ".srt")).MustHaveHappened();
        }
        
        [Test, AutoFakeData]
        public void TryDownloadSubtitle_DownloadSuccessful_ReturnsTrue(
            Subtitle subtitle,
            string resultFile,
            string fileName,
            [Frozen]ISubtitleDownloader downloader,
            SubtitleDownloaderWrapper sut)
        {
            A.CallTo(() => downloader.SaveSubtitle(A<Subtitle>._)).Returns(new [] { new FileInfo(resultFile) });
            
            var result = sut.TryDownloadSubtitle(subtitle, fileName);

            Assert.That(result, Is.True);
        }

        [Test, AutoFakeData]
        public void CanHandleAtLeastOneOf_NoLanguages_ReturnsFalse(
            SubtitleDownloaderWrapper sut)
        {
            bool result = sut.CanHandleAtLeastOneOf(new Language[0]);

            Assert.That(result, Is.True);
        }
        
        [Test, AutoFakeData]
        public void CanHandleAtLeastOneOf_HasOneOfTheLanguages_ReturnsTrue(
            Language handledLanguage,
            Language unhandledLanguage,
            [Frozen]ISubtitleDownloader downloader,
            SubtitleDownloaderWrapper sut)
        {
            A.CallTo(() => downloader.SupportedLanguages).Returns(new[] {handledLanguage});

            bool result = sut.CanHandleAtLeastOneOf(new[]{ unhandledLanguage, handledLanguage });

            Assert.That(result, Is.True);
        }
        
        [Test, AutoFakeData]
        public void CanHandleAtLeastOneOf_DoesNotHaveLanguage_ReturnsFalse(
            Language[] downloaderLanguages,
            Language[] otherLanguages,
            [Frozen]ISubtitleDownloader downloader,
            SubtitleDownloaderWrapper sut)
        {
            A.CallTo(() => downloader.SupportedLanguages).Returns(downloaderLanguages); 
            
            bool result = sut.CanHandleAtLeastOneOf(otherLanguages);

            Assert.That(result, Is.False);
        }

        [Test, AutoFakeData]
        public void CanHandleAtLeastOneOf_DoesNotHaveSupportedLanguages_ReturnsFalse(
            Language[] anyLanguages,
            [Frozen]ISubtitleDownloader downloader,
            SubtitleDownloaderWrapper sut)
        {
            A.CallTo(() => downloader.SupportedLanguages).Returns(new Language[0]);

            bool result = sut.CanHandleAtLeastOneOf(anyLanguages);

            Assert.That(result, Is.False);
        }
    }
}
