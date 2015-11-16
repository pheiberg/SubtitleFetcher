using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using SubtitleFetcher.Common.Downloaders.S4U;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.S4U
{
    [TestFixture]
    public class LimitsBuilderTests
    {
        [Test, AutoFakeData]
        public void BuildString_NoLimits_EmptyString(
            int season,
            LimitsBuilder sut)
        {
            var limits = new S4ULimits();

            var result = sut.BuildString(limits);

            Assert.That(result, Is.Empty);
        }
        
        [Test]
        [TestCase(1, null, null, null, "/Season=1")]
        [TestCase(1, 2, null, null, "/Season=1&Episode=2")]
        [TestCase(1, 2, 3, null, "/Season=1&Episode=2&Limit=3")]
        [TestCase(1, 2, 3, 4, "/Season=1&Episode=2&Limit=3&Year=4")]
        [TestCase(null, 1, 2, 3, "/Episode=1&Limit=2&Year=3")]
        [TestCase(null, 1, 2, null, "/Episode=1&Limit=2")]
        [TestCase(null, 1, null, null, "/Episode=1")]
        [TestCase(null, null, 1, null, "/Limit=1")]
        [TestCase(null, null, null, 1, "/Year=1")]
        public void BuildString_Scenarios_StringMatches(int? season, int? episode, int? limit, int? year, string expected)
        {
            var limitsBuilder = new LimitsBuilder();
            var limits = new S4ULimits { Season = season, Episode = episode, Limit = limit, Year = year};

            var result = limitsBuilder.BuildString(limits);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
