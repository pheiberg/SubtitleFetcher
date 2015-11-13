using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common
{
    public class Subtitle
    {
        public Subtitle(string id, string fileName, Language language)
        {
            Id = id;
            FileName = fileName;
            Language = language;
        }
        
        public string FileName { get; set; }
        public Language Language { get; set; }
        public string Id { get; set; }
        public string SeriesName { get; set; }
        public int? Season { get; set; }
        public int? Episode { get; set; }
        public int? EndEpisode { get; set; }
        public string ReleaseGroup { get; set; }
    }
}