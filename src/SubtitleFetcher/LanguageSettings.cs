namespace SubtitleFetcher
{
    public class LanguageSettings
    {
        private readonly string[] languages;

        public LanguageSettings(string[] languages)
        {
            this.languages = languages;
        }

        public string[] Languages
        {
            get { return languages; }
        }
    }
}