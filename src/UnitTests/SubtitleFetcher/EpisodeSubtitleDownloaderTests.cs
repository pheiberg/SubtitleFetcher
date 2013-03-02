using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using FakeItEasy;
using NUnit.Framework;
using SubtitleDownloader.Core;
using SubtitleFetcher;
using SubtitleFetcher.Common;

namespace UnitTests.SubtitleFetcher
{
    [TestFixture]
    public class EpisodeSubtitleDownloaderTests
    {
        [Test]
        public void SearchSubtitle_ThrowsException_ReturnEmptyResultSet()
        {
            var downloader = A.Fake<ISubtitleDownloader>();
            A.CallTo(() => downloader.SearchSubtitles(A<EpisodeSearchQuery>._)).Throws<Exception>();
            var nameParser = A.Fake<IEpisodeParser>();
            var logger = A.Fake<ILogger>();
            var fileSystem = A.Fake<IFileSystem>();
            var episodeDownloader = new EpisodeSubtitleDownloader(downloader, nameParser, logger, fileSystem);

            var results = episodeDownloader.SearchSubtitle(new EpisodeIdentity(), new string[0]);

            Assert.That(results, Is.Empty);
        }
        
        [Test]
        public void SearchSubtitle_MultipleValidSubtitlesFound_OrderedByLanguagePriority()
        {
            var episodeIdentity = new EpisodeIdentity("Show", 1, 1, "group");
            var subtitles = new List<Subtitle>()
            {
                new Subtitle("anything", "anything", episodeIdentity.ToString(), "eng"),
                new Subtitle("anything", "anything", episodeIdentity.ToString(), "dan"),
                new Subtitle("anything", "anything", episodeIdentity.ToString(), "swe")
            };

            var downloader = A.Fake<ISubtitleDownloader>();
            A.CallTo(() => downloader.SearchSubtitles(A<EpisodeSearchQuery>._)).Returns(subtitles);
            var nameParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var fileSystem = A.Fake<IFileSystem>();
            var episodeDownloader = new EpisodeSubtitleDownloader(downloader, nameParser, logger, fileSystem);
            var languages = new [] { "swe", "ger", "dan", "eng" };

            var results = episodeDownloader.SearchSubtitle(episodeIdentity, languages);

            Assert.That(results.Select(s => s.LanguageCode), Is.EqualTo(new [] { "swe", "dan", "eng" }));
        }
        
        [Test]
        public void SearchSubtitle_NonEquivalentSubtitlesFound_OnlyIncludesEquivalent()
        {
            var episodeIdentity = new EpisodeIdentity("Show", 1, 1, "group");
            var subtitles = new List<Subtitle>
                                {
                new Subtitle("anything", "anything", episodeIdentity.ToString(), "eng"),
                new Subtitle("anything", "anything", episodeIdentity.ToString(), "dan"),
                new Subtitle("anything", "anything", "Another Show S01 E01", "swe")
            };

            var downloader = A.Fake<ISubtitleDownloader>();
            A.CallTo(() => downloader.SearchSubtitles(A<EpisodeSearchQuery>._)).Returns(subtitles);
            var nameParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var fileSystem = A.Fake<IFileSystem>();
            var episodeDownloader = new EpisodeSubtitleDownloader(downloader, nameParser, logger, fileSystem);
            var languages = new [] { "swe", "ger", "dan", "eng" };

            var results = episodeDownloader.SearchSubtitle(episodeIdentity, languages);

            Assert.That(results.Select(s => s.FileName), Has.All.StringStarting("Show"));
        }
        
        [Test]
        public void TryDownloadSubtitle_DownloaderThrowsException_ReturnsFalse()
        {
            var downloader = A.Fake<ISubtitleDownloader>();
            A.CallTo(() => downloader.SaveSubtitle(A<Subtitle>._)).Throws<Exception>();
            var nameParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var fileSystem = A.Fake<IFileSystem>();
            var episodeDownloader = new EpisodeSubtitleDownloader(downloader, nameParser, logger, fileSystem);

            bool result = episodeDownloader.TryDownloadSubtitle(new Subtitle("anything", "anything", "anything", "swe"), "targetFileName");

            Assert.That(result, Is.False);
        }

        [Test]
        public void TryDownloadSubtitle_DownloadSuccessful_RenamesFile()
        {
            var downloader = A.Fake<ISubtitleDownloader>();
            var fileInfo = new FileInfo("fileName");
            A.CallTo(() => downloader.SaveSubtitle(A<Subtitle>._)).Returns(new List<FileInfo> { fileInfo });
            var nameParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var fileSystem = A.Fake<IFileSystem>();
            var episodeDownloader = new EpisodeSubtitleDownloader(downloader, nameParser, logger, fileSystem);

            episodeDownloader.TryDownloadSubtitle(new Subtitle("anything", "anything", "anything", "swe"), "targetFileName");

            A.CallTo(() => fileSystem.RenameSubtitleFile(fileInfo.FullName, "targetFileName.swe.srt")).MustHaveHappened();
        }
        
        [Test]
        public void TryDownloadSubtitle_DownloadSuccessful_ReturnsTrue()
        {
            var downloader = A.Fake<ISubtitleDownloader>();
            A.CallTo(() => downloader.SaveSubtitle(A<Subtitle>._)).Returns(new List<FileInfo>{new FileInfo("anything")});
            var nameParser = new EpisodeParser();
            var logger = A.Fake<ILogger>();
            var fileSystem = A.Fake<IFileSystem>();
            var episodeDownloader = new EpisodeSubtitleDownloader(downloader, nameParser, logger, fileSystem);
            
            var result = episodeDownloader.TryDownloadSubtitle(new Subtitle("anything", "anything", "anything", "swe"), "targetFileName");

            Assert.That(result, Is.True);
        }

        [Test]
        public void CanHandleAtLeastOneOf_NotExtended_ReturnsTrue()
        {
            var downloader = A.Fake<ISubtitleDownloader>();
            var episodeDowloader = new EpisodeSubtitleDownloader(downloader, null, null, null);

            bool result = episodeDowloader.CanHandleAtLeastOneOf(new string[0]);

            Assert.That(result, Is.True);
        }
        
        [Test]
        public void CanHandleAtLeastOneOf_ExtendedCanHandle_ReturnsTrue()
        {
            var downloader = A.Fake<IExtendedSubtitleDownloader>();
            A.CallTo(() => downloader.LanguageLimitations).Returns(new[] { "swe" });
            var episodeDowloader = new EpisodeSubtitleDownloader(downloader, null, null, null);

            bool result = episodeDowloader.CanHandleAtLeastOneOf(new[]{ "eng", "swe" });

            Assert.That(result, Is.True);
        }
        
        [Test]
        public void CanHandleAtLeastOneOf_ExtendedCanNotHandle_ReturnsFalse()
        {
            var downloader = A.Fake<IExtendedSubtitleDownloader>();
            A.CallTo(() => downloader.LanguageLimitations).Returns(new[] { "swe" }); 
            var episodeDowloader = new EpisodeSubtitleDownloader(downloader, null, null, null);

            bool result = episodeDowloader.CanHandleAtLeastOneOf(new [] { "eng" });

            Assert.That(result, Is.False);
        }
    }
}
