using NUnit.Framework;
using SubtitleFetcher;

namespace UnitTests
{
    [TestFixture]
    public class TVDBTests
    {
        [Test]
        public void GetSeries_ExactMatch_ReturnsOneResult()
        {
            var searcher = new TvdbSearcher();
            var result = searcher.FindSeries("Supernatural");
        }
    }
}
