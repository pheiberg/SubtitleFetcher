using System;
using System.Text.RegularExpressions;

namespace SubtitleFetcher.Common
{
    public class EpisodeParser : IEpisodeParser
    {
        readonly string[] patterns = new[]
                                         {
                                             @"^(?<SeriesName>[\w\s\._]+?)S(?<Season>\d\d?)E(?<Episode>\d\d?).*?(-(?<ReleaseGroup>\w+))?$",
                                             @"^(?<SeriesName>[\w\s\._]+?)S(?<Season>\d\d?)E(?<Episode>\d\d?).*?(\[(?<ReleaseGroup>\w+)\])?$",
                                             @"^(?<SeriesName>[\w\s\._]+?)(?<Season>\d\d?)X(?<Episode>\d\d?).*?(\[(?<ReleaseGroup>\w+)\])$",
                                             @"^(?<SeriesName>[\w\s\._]+?)(?<Season>\d\d?)X(?<Episode>\d\d?).*?(-(?<ReleaseGroup>\w+))?$"
                                         };
        public EpisodeIdentity ParseEpisodeInfo(string fileName)
        {
            foreach (var pattern in patterns)
            {
                var match = Regex.Match(fileName, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                if (!match.Success) 
                    continue;

                var seriesName = match.Groups["SeriesName"].Value.Replace('.', ' ').Replace('_', ' ').Trim();
                var season = int.Parse(match.Groups["Season"].Value);
                var episode = int.Parse(match.Groups["Episode"].Value);
                var releaseGroup = match.Groups["ReleaseGroup"].Value;
                return new EpisodeIdentity { SeriesName = seriesName, Season = season, Episode = episode, ReleaseGroup = releaseGroup};
            }
            return new EpisodeIdentity();
        }
    }
}