namespace PortedSubtitleDownloaders.Legacy
{
    public class EpisodeSearchQuery : SubtitleSearchQuery
    {
        public string SerieTitle { get; }

        public int Episode { get; }

        public int Season { get; }

        public int? TvdbId { get; private set; }

        public EpisodeSearchQuery(string serieTitle, int season, int episode, int? tvdbId = null)
        {
            SerieTitle = serieTitle;
            Season = season;
            Episode = episode;
            TvdbId = tvdbId;
        }
    }
}