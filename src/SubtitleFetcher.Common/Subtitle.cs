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
        
        public string Release { get; set; }
        public string FileName { get; set; }
        public Language Language { get; set; }
        public string Id { get; set; }
    }
}