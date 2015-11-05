﻿using System.IO;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Hashing;

namespace UnitTests.SubtitleFetcher.Common
{
    [TestFixture]
    public class FileHasherTests
    {
        [Test, AutoFakeData]
        public void Create64kHash_128kStream_ReturnsHashFromHashAlgo(
            byte[] expectedHash,
            [Frozen]IHashCalculator hashProvider,
            FileHasher sut)
        {
            var data = CreateData();
            A.CallTo(() => hashProvider.ComputeHash(
                A<byte[]>.That.IsSameSequenceAs(data)))
               .Returns(expectedHash);

            using (var stream = new MemoryStream(data))
            {
                var result = sut.CreateHash(stream);
                Assert.That(result, Is.EquivalentTo(expectedHash));
            }
        }
        
        private static byte[] CreateData()
        {
            return new Fixture().CreateMany<byte>(128 * 1024).ToArray();
        }   
    }
}
