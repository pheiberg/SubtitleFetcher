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
            var d = new SubDbApi();
            var file = d.DownloadSubtitle("edc1981d6459c6111fe36205b4aff6c2", new[] {"en"});
            Assert.That(file.Length, Is.EqualTo(157));
        }

        [Test]
        public void Search()
        {
            var expected = "en,fr,it,pt".Split(',');
            var d = new SubDbApi();
            var result = d.Search("edc1981d6459c6111fe36205b4aff6c2");
            Assert.That(result, Is.EquivalentTo(expected));
        }
    }
}