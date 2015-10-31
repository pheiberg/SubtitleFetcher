namespace SubtitleFetcher.Common
{
    public class SearchQuery
    {
        public SearchQuery(string seriesName, int season, int episode)
        {
            SerieTitle = seriesName;
            Season = season;
            Episode = episode;
        }

        public string SerieTitle { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public string[] LanguageCodes { get; set; }
    }
}