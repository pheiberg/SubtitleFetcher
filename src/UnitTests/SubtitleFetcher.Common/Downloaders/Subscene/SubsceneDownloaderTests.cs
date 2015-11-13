using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders.Subscene;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Parsing;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.Subscene
{
    [TestFixture]
    public class SubsceneDownloaderTests
    {
        [Test]
        public void Search()
        {
            var downloader = new SubsceneDownloader(new EpisodeParser());

            var languages = new []{ KnownLanguages.GetLanguageByTwoLetterIso("sv") };
            var results = downloader.SearchSubtitles(new SearchQuery("Spartacus", 3, 2, "AFG") { Languages = languages });

            Assert.That(results, Is.Not.Empty);
        }
    }
}
