using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SubtitleFetcher.Common.Parsing
{
    public class EpisodeParser : IEpisodeParser
    {
        private static readonly string[] RecognizedTags = 
            { "PROPER", "REPACK", "RERIP", "720p", "1080p", "WEB-DL",
              "H264", "x264", "H.264", "HDTV", "DD51", "DD5.1", "AAC2.0",
              "AAC20", "DL" };

        private static readonly string TagsPattern = CreateTagsPattern(RecognizedTags);

        private static string CreateTagsPattern(string[] recognizedTags)
        {
            string tags = string.Join("|", recognizedTags.Select(tag => $"({tag})"));
            return $"(?<Tags>({tags}))*";
        }

        readonly string[] _patterns = {
                                        @"^((?<SeriesName>.+?)[\[. _-]+)?(?<Season>\d+)x(?<Episode>\d+)(([. _-]*x|-)(?<EndEpisode>(?!(1080|720)[pi])(?!(?<=x)264)\d+))*[\]. _-]*((?<ExtraInfo>.+?)((?<![. _-])-(?<ReleaseGroup>[^-]+))?)?$",
                                        @"^((?<SeriesName>.+?)[. _-]+)?s(?<Season>\d+)[. _-]*e(?<Episode>\d+)(([. _-]*e|-)(?<EndEpisode>(?!(1080|720)[pi])\d+))*[. _-]*((?<ExtraInfo>.+?)((?<![. _-])-(?<ReleaseGroup>[^-]+))?)?$"
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
                var endEpisode = ExtractEndEpisode(match.Groups["EndEpisode"], episode);
                var releaseGroup = match.Groups["ReleaseGroup"].Value;
                var extraInfo = GetTags(match.Groups["ExtraInfo"]);

                var releaseIdentity = new TvReleaseIdentity
                {
                    SeriesName = seriesName,
                    Season = season,
                    Episode = episode,
                    EndEpisode = endEpisode,
                    ReleaseGroup = releaseGroup
                };
                foreach (var tag in extraInfo)
                {
                    releaseIdentity.Tags.Add(tag);
                }

                return releaseIdentity;
            }
            return new TvReleaseIdentity();
        }

        private static int ExtractEndEpisode(Capture endEpisodeGroup, int episode)
        {
            return !string.IsNullOrEmpty(endEpisodeGroup.Value) ? int.Parse(endEpisodeGroup.Value) : episode;
        }

        private static IEnumerable<string> GetTags(Capture extraInfoGroup)
        {
            char[] separators = { '.', ' ', '-', '_' };
            var extraInfo = extraInfoGroup.Value;
            var tags = extraInfo.Split(separators);
            return tags.Select(e => e.Trim());
        }
    }
}