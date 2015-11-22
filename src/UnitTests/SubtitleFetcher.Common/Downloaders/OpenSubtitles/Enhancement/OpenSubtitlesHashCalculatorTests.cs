using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement
{
    [TestFixture]
    public class OpenSubtitlesHashCalculatorTests
    {
        [Test, AutoFakeData]
        public void ComputeHash_EmptyInput_ReturnsEmptyHash(
            OpenSubtitlesHashCalculator sut)
        {
            var bytes = new byte[0];

            var result = sut.ComputeHash(bytes);

            Assert.That(result, Is.Empty);
        }

        [Test, AutoFakeData]
        public void ComputeHash_InputSmallerThanBlockSize_ReturnsEmptyHash(
            OpenSubtitlesHashCalculator sut)
        {
            var fixture = new Fixture { RepeatCount = new Random().Next(0, 63) };
            var bytes = fixture.Create<byte[]>();

            var result = sut.ComputeHash(bytes);

            Assert.That(result, Is.Empty);
        }

        public void ComputeHash_InputLargerThanBlockSize_ReturnsNonEmptyHash(
            OpenSubtitlesHashCalculator sut)
        {
            var fixture = new Fixture { RepeatCount = new Random().Next(64, int.MaxValue) };
            var bytes = fixture.Create<byte[]>();

            var result = sut.ComputeHash(bytes);

            Assert.That(result, Is.Not.Empty);
        }
    }
}
