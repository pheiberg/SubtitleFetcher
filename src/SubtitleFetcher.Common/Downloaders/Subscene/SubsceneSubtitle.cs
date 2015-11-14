namespace SubtitleFetcher.Common.Downloaders.Subscene
{
    public class SubsceneSubtitle
    {
        public string SubtitleLink { get; set; }
        public string LanguageName { get; set; }
        public string SeriesName { get; set; }
        public int? Season { get; set; }
        public int? Episode { get; set; }
        public int? EndEpisode { get; set; }
        public string ReleaseGroup { get; set; }
        public string ReleaseName { get; set; }
        public int RatingType { get; set; }
    }
}