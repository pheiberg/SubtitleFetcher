using System.Collections.Generic;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public interface ISubtitleFilter
    {
        IEnumerable<Subtitle> Apply(IEnumerable<Subtitle> subtitles, SearchQuery query);
    }
}