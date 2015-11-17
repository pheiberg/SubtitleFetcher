using System.Linq;
using NUnit.Framework;
using SubtitleFetcher.Common.Downloaders.S4U;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.S4U
{
    [TestFixture]
    public class S4UApiTests
    {
        [Test]
        public void SearchByTitle_TitleExists_ContainsSubs()
        {
            var api = new S4UApi(new S4USettings {ApiKey = "ApiTestKey" });

            var result = api.SearchByTitle("Heroes", new S4ULimits { Season = 1, Episode = 1});

            Assert.That(result.Series.First().Subs, Is.Not.Empty);
        }

        [Test]
        public void SearchByRelease_ReleaseExists_ContainsSubs()
        {
            var api = new S4UApi(new S4USettings { ApiKey = "ApiTestKey" });

            var result = api.SearchByRelease("Heroes.S01E01.HDTV.XviD-LOL", new S4ULimits { Season = 1, Episode = 1 });

            Assert.That(result.Series.First().Subs, Is.Not.Empty);
        }

        [Test]
        public void SearchByImdbId_ReleaseExists_ContainsSubs()
        {
            var api = new S4UApi(new S4USettings { ApiKey = "ApiTestKey" });

            var result = api.SearchByImdbId(813715, new S4ULimits { Season = 1, Episode = 1 });

            Assert.That(result.Series.First().Subs, Is.Not.Empty);
        }

        [Test]
        public void SearchByTvdbId_ReleaseExists_ContainsSubs()
        {
            var api = new S4UApi(new S4USettings { ApiKey = "ApiTestKey" });

            var result = api.SearchByTvdbId(79501, null, new S4ULimits { Season = 1, Episode = 1 });

            Assert.That(result.Series.First().Subs, Is.Not.Empty);
        }


        [Test]
        public void SearchByTvdbIdAndEpisodeId_ReleaseExists_ContainsSubs()
        {
            var api = new S4UApi(new S4USettings { ApiKey = "ApiTestKey" });

            var limits = new S4ULimits();
            limits.Custom.Add("tvdb_ep_id", "308907");

            var result = api.SearchByTvdbId(79501, null, limits);

            Assert.That(result.Series.First().Subs, Is.Not.Empty);
        }
    }
}
