using System.Collections.Generic;

namespace TvShowIdentification
{
    public interface ITvdbSearcher
    {
        IEnumerable<TvdbSeries> FindSeries(string name);
        TvdbSeries FindSeriesExact(string name);
    }
}