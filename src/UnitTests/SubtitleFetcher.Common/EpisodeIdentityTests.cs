﻿using NUnit.Framework;
using SubtitleFetcher.Common;

namespace UnitTests.SubtitleFetcher.Common
{
    [TestFixture]
    public class EpisodeIdentityTests
    {
        [Test]
        [TestCase("Mr Selfridge", "MR Selfridge", true)]
        [TestCase("MrSelfridge", "Mr Selfridge", true)]
        [TestCase("Mr.Selfridge", "Mr Selfridge", true)]
        [TestCase("Mr_Selfridge", "Mr Selfridge", true)]
        [TestCase("Mister Selfridge", "Mr Selfridge", false)]
        public void IsEquivalent(string name1, string name2, bool expected)
        {
            var id1 = new TvReleaseIdentity { SeriesName = name1 };
            var id2 = new TvReleaseIdentity { SeriesName = name2 };
            Assert.That(id1.IsEquivalent(id2), Is.EqualTo(expected));
        }
    }
}