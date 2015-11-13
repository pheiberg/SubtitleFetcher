using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher.Common.Orchestration
{
    public class SubtitleMatcher
    {
        private readonly IEpisodeParser _episodeParser;

        public SubtitleMatcher(IEpisodeParser episodeParser)
        {
            _episodeParser = episodeParser;
        }

        public IEnumerable<Subtitle> FilterOutSubtitlesNotMatching(IEnumerable<Subtitle> subtitles, TvReleaseIdentity identity)
        {
            return from subtitle in subtitles
                let subtitleInfo = _episodeParser.ExtractReleaseIdentity(subtitle)
                where subtitleInfo.IsEquivalent(identity)
                select subtitle;
        }
    }
}