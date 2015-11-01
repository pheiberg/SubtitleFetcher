namespace SubtitleFetcher.Common
{
    public class Subtitle
    {
        public Subtitle(string id, string programName, string fileName, string languageCode)
        {
            Id = id;
            FileName = fileName;
            ProgramName = programName;
            LanguageCode = languageCode;
        }

        public string ProgramName { get; set; }

        public string Release { get; set; }

        public string FileName { get; set; }
        public string LanguageCode { get; set; }
        public string Id { get; set; }
    }
}