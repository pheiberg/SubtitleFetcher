﻿using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Orchestration;
using SubtitleFetcher.Common.Parsing;

namespace UnitTests.SubtitleFetcher.Common.Orchestration
{
    [TestFixture]
    public class SubtitleMatcherTests
    {
        [Test, AutoFakeData]
        public void FilterOutSubtitlesNotMatching_SomeMatching_AllMatchingAreLeft(
            IEnumerable<Subtitle> subtitles, 
            TvReleaseIdentity identity,
            [Frozen]IEpisodeParser parser,
            SubtitleMatcher sut)
        {
            var matching = subtitles.Take(2);
            var matchingFileNames = matching.Select(m => m.FileName);
            A.CallTo(() => parser.ParseEpisodeInfo(A<string>.That.Matches(s => matchingFileNames.Contains(s))))
                .Returns(identity);

            var results = sut.FilterOutSubtitlesNotMatching(subtitles, identity);

            Assert.That(results.Select(r => r.Id), Is.EquivalentTo(matching.Select(s => s.Id)));
        }
    }
}
