using NUnit.Framework;
using SubtitleFetcher.Common.Downloaders.SubDb;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.SubDb
{
    [TestFixture]
    public class SubDbDownloaderTests
    {
        [Test]
        public void Download()
        {
            var d = new SubDbDownloader();
            var file = d.DownloadSubtitle("edc1981d6459c6111fe36205b4aff6c2", new[] {"en"});
        } 
    }
}