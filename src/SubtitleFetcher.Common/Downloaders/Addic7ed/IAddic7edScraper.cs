using System.Collections.Generic;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public interface IAddic7edScraper
    {
        int? FindSeries(string seriesName);
        IEnumerable<Addic7edSubtitle> SearchSubtitles(int seriesId, int season);
    }
}