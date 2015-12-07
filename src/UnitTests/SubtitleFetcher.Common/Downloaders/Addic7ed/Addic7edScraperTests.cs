using System.Linq;
using NUnit.Framework;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders.Addic7ed;
using SubtitleFetcher.Common.Languages;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.Addic7ed
{
    [TestFixture]
    public class Addic7edScraperTests
    {
        [Test, AutoFakeData]
        public void FindSeries_ShowExists_ReturnsCorrectId(Addic7edScraper sut)
        {
            int? result = sut.FindSeries("ALF");

            Assert.That(result, Is.EqualTo(280));
        }

        [Test, AutoFakeData]
        public void FindSeries_ShowDoesNotExist_ReturnsNull(Addic7edScraper sut)
        {
            int? result = sut.FindSeries("NoneExistant");

            Assert.That(result, Is.EqualTo(null));
        }

        [Test, AutoFakeData]
        public void SearchSubtitles_EpisodeThatHasReleases_ReturnsReleasesWithCorrectData(Addic7edScraper sut)
        {
            var english = KnownLanguages.GetLanguageByName("English");

            var results = sut.SearchSubtitles(15, "House", 5, 1, new []{ english }).ToArray();

            Assert.That(results, Has.All.Matches<Subtitle>(s => 
                s.SeriesName == "House" &&
                s.Season == 5 &&
                s.Episode == 1 &&
                s.EndEpisode == 1 &&
                s.Language == english
                ));
        }
    }
}
