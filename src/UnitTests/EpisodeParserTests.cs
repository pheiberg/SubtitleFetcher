using NUnit.Framework;
using SubtitleFetcher;
using TvShowIdentification;

namespace UnitTests
{
    [TestFixture]
    public class EpisodeParserTests
    {
        [Test]
        [TestCase("Mr Selfridge 1x07 HDTV x264-FoV", "Mr Selfridge", 1, 7, "FoV")]
        [TestCase("Mr Selfridge S01E02 720p HDTV x264-TLA", "Mr Selfridge", 1, 2, "TLA")]
        [TestCase("Mr_Selfridge 1x07 HDTV_x264-FoV", "Mr Selfridge", 1, 7, "FoV")]
        [TestCase("MrSelfridge.s01e01.pdtv.xvid-hannibal", "MrSelfridge", 1, 1, "hannibal")]
        [TestCase("Glee 3x14 (HDTV-LOL) [VTV]", "Glee", 3, 14, "VTV")]
        [TestCase("Supernatural S07E01 PROPER REPACK 720p HDTV x264-ORENJI", "Supernatural", 7, 1, "ORENJI")]
        [TestCase("The Big Bang Theory S05E13 The Recombination Hypothesis PROPER HDTV XviD-FQM", "The Big Bang Theory", 5, 13, "FQM")]
        [TestCase("Glee S03E14 Some title", "Glee", 3, 14, "")]
        [TestCase("Glee 3X14 Some title", "Glee", 3, 14, "")]
        [TestCase("Mythbusters S05E04 Speed Cameras WS DSR XviD-2SD", "Mythbusters", 5, 4, "2SD")]
        public void Parse_ParsableName_PopulatedEpisodeInfo(string name, string seriesName, int season, int episode, string releaseGroup)
        {
            var exected = new EpisodeIdentity {SeriesName = seriesName, Season = season, Episode = episode, ReleaseGroup = releaseGroup};
            var parser = new EpisodeParser();
            var info = parser.ParseEpisodeInfo(name);
            Assert.That(info, Is.EqualTo(exected));
        }
    }
}
