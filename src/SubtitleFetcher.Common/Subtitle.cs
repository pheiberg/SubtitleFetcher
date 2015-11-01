namespace SubtitleFetcher.Common
{
    public class Subtitle
    {
        public Subtitle(string id, string title, string fileName, string languageCode)
        {
            Id = id;
            FileName = fileName;
            Title = title;
            LanguageCode = languageCode;
        }

        public string Title { get; set; }
        public string Release { get; set; }
        public string FileName { get; set; }
        public string LanguageCode { get; set; }
        public string Id { get; set; }
    }
}