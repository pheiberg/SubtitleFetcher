using System;

namespace SubtitleFetcher.Common.Enhancement.Tvdb
{
    public class TvDbEnhancer : IEnhancer
    {
        readonly TvdbSearcher _searcher = new TvdbSearcher();

        public IEnhancement Enhance(string filePath, TvReleaseIdentity identity)
        {
            var series = _searcher.FindSeriesExact(identity.SeriesName);
            return series.SeriesId != null ? new TvDbEnhancement(series.SeriesId.Value) : null;
        }

        public Type ProvidedEnhancement => typeof(TvDbEnhancement);
    }
}
