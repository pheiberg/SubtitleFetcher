using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders.Addic7ed;

namespace UnitTests.SubtitleFetcher.Common.Downloaders.Addic7ed
{
    [TestFixture]
    public class Addic7edDownloaderTests
    {
        [Test, AutoFakeData]
        public void SearchSubtitles_SeriesFound_ReturnsResultsFromScraper(
            int? seriesId,
            IEnumerable<Addic7edSubtitle> subtitles,
            IEnumerable<Subtitle> expected,
            SearchQuery query,
            [Frozen]IAddic7edScraper scraper,
            [Frozen]ISubtitleFilter filter,
            [Frozen]ISubtitleMapper mapper,
            [Greedy]Addic7edDownloader sut
            )
        {
            A.CallTo(() => scraper.FindSeries(query.SeriesTitle)).Returns(seriesId);
            A.CallTo(() => scraper.SearchSubtitles(seriesId.Value, query.Season)).Returns(subtitles);
            A.CallTo(() => mapper.Map(subtitles, query.SeriesTitle)).Returns(expected);
            A.CallTo(() => filter.Apply(A<IEnumerable<Subtitle>>.Ignored, query)).Returns(expected);

            var result = sut.SearchSubtitles(query);

            Assert.That(result.Select(s => s.Id), Is.EquivalentTo(expected.Select(s => s.Id)));
        }

        [Test, AutoFakeData]
        public void SearchSubtitles_SeriesNotFound_ReturnsEmpty(
            IEnumerable<Subtitle> notExpected, 
            SearchQuery query,
            [Frozen]IAddic7edScraper scraper,
            [Frozen]ISubtitleFilter filter,
            [Greedy]Addic7edDownloader sut
            )
        {
            A.CallTo(() => scraper.FindSeries(query.SeriesTitle)).Returns(null);
            A.CallTo(() => filter.Apply(A<IEnumerable<Subtitle>>.Ignored, A<SearchQuery>.Ignored)).Returns(notExpected);

            var result = sut.SearchSubtitles(query);

            Assert.That(result, Is.Empty);
        }

        [Test, AutoFakeData]
        public void SearchSubtitles_SeriesFoundFilterApplied_ReturnsFilterResults(
            int? seriesId,
            IEnumerable<Subtitle> expected,
            SearchQuery query,
            [Frozen]IAddic7edScraper scraper,
            [Frozen]ISubtitleFilter filter,
            [Greedy]Addic7edDownloader sut
            )
        {
            A.CallTo(() => scraper.FindSeries(query.SeriesTitle)).Returns(seriesId);
            A.CallTo(() => filter.Apply(A<IEnumerable<Subtitle>>.Ignored, query)).Returns(expected);  

            var result = sut.SearchSubtitles(query);

            Assert.That(result.Select(s => s.Id), Is.EquivalentTo(expected.Select(s => s.Id)));
        }
    }
}