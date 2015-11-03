using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using SubtitleFetcher.Common;

namespace UnitTests.SubtitleFetcher.Common
{
    [TestFixture]
    public class EpisodeParserTests
    {
        [Test]
        [TestCase("Mr Selfridge 1x07 HDTV x264-FoV", "Mr Selfridge", 1, 7, 7, "FoV")]
        [TestCase("Mr Selfridge S01E02 720p HDTV x264-TLA", "Mr Selfridge", 1, 2, 2, "TLA")]
        [TestCase("Mr_Selfridge 1x07 HDTV_x264-FoV", "Mr Selfridge", 1, 7, 7,"FoV")]
        [TestCase("MrSelfridge.s01e01.pdtv.xvid-hannibal", "MrSelfridge", 1, 1, 1,"hannibal")]
        [TestCase("Glee 3x14 (HDTV-LOL) [VTV]", "Glee", 3, 14, 14,"VTV")]
        [TestCase("Supernatural S07E01 PROPER REPACK 720p HDTV x264-ORENJI", "Supernatural", 7, 1, 1, "ORENJI")]
        [TestCase("The Big Bang Theory S05E13 The Recombination Hypothesis PROPER HDTV XviD-FQM", "The Big Bang Theory", 5, 13, 13, "FQM")]
        [TestCase("Glee S03E14 Some title", "Glee", 3, 14, 14, "")]
        [TestCase("Glee 3X14 Some title", "Glee", 3, 14, 14, "")]
        [TestCase("Mythbusters S05E04 Speed Cameras WS DSR XviD-2SD", "Mythbusters", 5, 4, 4, "2SD")]
        [TestCase("be.cool.scooby-doo.s01e01.mystery.101.hdtv.x264-w4f", "be cool scooby-doo", 1, 1, 1, "w4f")]
        [TestCase("MultiEp.S01E01-E03.HDTV.x264-Group", "MultiEp", 1, 1, 3, "Group")]
        [TestCase("Family.Guy.S14E02.Papa.Has.a.Rollin.Son.1080p.WEB-DL.DD5.1.H.264-CtrlHD", "Family Guy", 14, 2, 2, "CtrlHD")]
        public void Parse_ParsableName_PopulatedEpisodeInfo(string name, string seriesName, int season, int episode, int endEpisodeNumber, string releaseGroup)
        {
            var expected = new TvReleaseIdentity {
                SeriesName = seriesName,
                Season = season,
                Episode = episode,
                EndEpisode = endEpisodeNumber,
                ReleaseGroup = releaseGroup
            };
            var parser = new EpisodeParser();

            var info = parser.ParseEpisodeInfo(name);

            Assert.That(info, Is.EqualTo(expected));
        }
    }
}
