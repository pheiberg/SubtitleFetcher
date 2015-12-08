using System.Linq;
using NUnit.Framework;
using SubtitleFetcher.Common.Downloaders.Addic7ed;

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
            var results = sut.SearchSubtitles(15, 5).ToArray();

            Assert.That(results, Has.All.Matches<Addic7edSubtitle>(s => 
                s.Season == 5));
        }
    }
}
