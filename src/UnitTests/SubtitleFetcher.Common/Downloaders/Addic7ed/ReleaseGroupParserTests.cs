using NUnit.Framework;
using SubtitleFetcher.Common.Downloaders.Addic7ed;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.Addic7ed
{
    [TestFixture]
    public class ReleaseGroupParserTests
    {
        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("NoTV", "NoTV")]
        [TestCase("720p CTU", "CTU")]
        [TestCase("720p-CTU", "CTU")]
        [TestCase("720p.WEB-DL.PeeWee", "PeeWee")]
        [TestCase("DVDRip.x264-DEMAND", "DEMAND")]
        [TestCase("HDTV-PROPER-x264-2HD", "2HD")]
        public void Parse_ReleaseGroupSamples_CorrectlyParsed(string source, string expected)
        {
            var sut = new ReleaseGroupParser();

            var result = sut.Parse(source);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
