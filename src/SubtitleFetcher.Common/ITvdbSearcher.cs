using System.Collections.Generic;

namespace SubtitleFetcher.Common
{
    public interface ITvdbSearcher
    {
        IEnumerable<TvdbSeries> FindSeries(string name);
        TvdbSeries FindSeriesExact(string name);
    }
}