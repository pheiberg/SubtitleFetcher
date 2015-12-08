using System.Collections.Generic;
using System.Linq;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class SubtitleFilter : ISubtitleFilter
    {
        public IEnumerable<Subtitle> Apply(IEnumerable<Subtitle> subtitles, SearchQuery query)
        {
            return subtitles.Where(s => query.Languages.Contains(s.Language)
                             && s.Season == query.Season
                             && s.Episode == query.Episode);
        }
    }
}