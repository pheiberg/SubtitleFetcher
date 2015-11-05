using NUnit.Framework;
using SubtitleFetcher.Common;

namespace UnitTests.SubtitleFetcher.Common
{
    [TestFixture]
    public class ByteExtensionTests
    {
        [Test]
        public void ToHexString_StaticData_CorrectHex()
        {
            var data = new byte[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};

            var result = data.ToHexString();

            Assert.That(result, Is.EqualTo("000102030405060708090A0B0C0D0E0F10"));
        }
    }
}
