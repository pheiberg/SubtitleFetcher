using System.Collections.Generic;

namespace SubtitleFetcher.Common.Enhancement.Tvdb
{
    public interface ITvdbSearcher
    {
        IEnumerable<TvdbSeries> FindSeries(string name);
        TvdbSeries FindSeriesExact(string name);
    }
}