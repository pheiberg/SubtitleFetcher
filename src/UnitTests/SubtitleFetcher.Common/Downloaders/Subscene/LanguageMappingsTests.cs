using NUnit.Framework;
using SubtitleFetcher.Common.Downloaders.Subscene;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.Subscene
{
    [TestFixture]
    public class LanguageMappingsTests
    {
        [Test]
        public void Map_All_AllLanguagesAreNotNull()
        {
            var languages = LanguageMappings.Map.Keys;

            Assert.That(languages, Has.No.Null);
        }
    }
}
