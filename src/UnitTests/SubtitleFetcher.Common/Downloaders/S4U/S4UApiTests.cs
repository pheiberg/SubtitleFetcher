using NUnit.Framework;
using SubtitleFetcher.Common.Downloaders.S4U;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.S4U
{
    [TestFixture]
    public class S4UApiTests
    {
        [Test]
        public void A()
        {
            var api = new S4UApi(new S4USettings {ApiKey = "ApiTestKey" });

            var result = api.SearchByTitle("Heroes", new S4ULimits { Season = 1, Episode = 1});

            Assert.That(result, Is.Not.Null);
        }
    }
}
