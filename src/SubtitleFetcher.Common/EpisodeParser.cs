using System;
using System.Text.RegularExpressions;

namespace SubtitleFetcher.Common
{
    public class EpisodeParser : IEpisodeParser
    {
        readonly string[] _patterns = {
                                             @"^(?<SeriesName>[\w\s\._\-]+?)S(?<Season>\d\d?)E(?<Episode>\d\d?)(-E(?<EndEpisode>\d\d?))?.*?(-(?<ReleaseGroup>\w+))?$",
                                             @"^(?<SeriesName>[\w\s\._\-]+?)S(?<Season>\d\d?)E(?<Episode>\d\d?)(-E(?<EndEpisode>\d\d?))?.*?(\[(?<ReleaseGroup>\w+)\])?$",
                                             @"^(?<SeriesName>[\w\s\._\-]+?)(?<Season>\d\d?)X(?<Episode>\d\d?).*?(\[(?<ReleaseGroup>\w+)\])$",
                                             @"^(?<SeriesName>[\w\s\._\-]+?)(?<Season>\d\d?)X(?<Episode>\d\d?).*?(-(?<ReleaseGroup>\w+))?$"
                                         };
        public TvReleaseIdentity ParseEpisodeInfo(string fileName)
        {
            foreach (var pattern in _patterns)
            {
                var match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                if (!match.Success) 
                    continue;

                var seriesName = match.Groups["SeriesName"].Value.Replace('.', ' ').Replace('_', ' ').Trim();
                var season = int.Parse(match.Groups["Season"].Value);
                var episode = int.Parse(match.Groups["Episode"].Value);
                var endEpisodeString = match.Groups["EndEpisode"].Value;
                var endEpisode = !string.IsNullOrEmpty(endEpisodeString) ? int.Parse(endEpisodeString) : episode;
                var releaseGroup = match.Groups["ReleaseGroup"].Value;
                return new TvReleaseIdentity
                {
                    SeriesName = seriesName,
                    Season = season,
                    Episode = episode,
                    EndEpisode = endEpisode,
                    ReleaseGroup = releaseGroup
                };
            }
            return new TvReleaseIdentity();
        }
    }
}