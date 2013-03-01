using System;
using System.Collections.Generic;
using System.Linq;
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
        public void SearchSubtitle_ReturnsMultipleSubtitles_OrderedByLanguagePriority()
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
    }
}
