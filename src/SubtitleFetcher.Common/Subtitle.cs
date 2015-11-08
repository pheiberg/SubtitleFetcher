namespace SubtitleFetcher.Common
{
    public class Subtitle
    {
        public Subtitle(string id, string title, string fileName, Language language)
        {
            Id = id;
            FileName = fileName;
            Title = title;
            Language = language;
        }

        public string Title { get; set; }
        public string Release { get; set; }
        public string FileName { get; set; }
        public Language Language { get; set; }
        public string Id { get; set; }
    }
}