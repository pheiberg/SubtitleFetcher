using System.Collections.Generic;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common
{
    public class SearchQuery
    {
        public SearchQuery(string seriesName, int season, int episode, string releaseGroup)
        {
            SerieTitle = seriesName;
            Season = season;
            Episode = episode;
            Enhancements = new List<IEnhancement>();
            ReleaseGroup = releaseGroup;
        }

        public string SerieTitle { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public Language[] Languages { get; set; }
        public string ReleaseGroup { get; set; }
        public IList<IEnhancement> Enhancements { get; } 
    }
}