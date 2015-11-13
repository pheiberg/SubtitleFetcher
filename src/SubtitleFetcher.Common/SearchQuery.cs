using System.Collections.Generic;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common
{
    public class SearchQuery
    {
        public SearchQuery(string seriesName, int season, int episode, string releaseGroup)
        {
            SeriesTitle = seriesName;
            Season = season;
            Episode = episode;
            Enhancements = new List<IEnhancement>();
            ReleaseGroup = releaseGroup;
        }

        public string SeriesTitle { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public Language[] Languages { get; set; }
        public string ReleaseGroup { get; set; }
        public IList<IEnhancement> Enhancements { get; } 
    }
}