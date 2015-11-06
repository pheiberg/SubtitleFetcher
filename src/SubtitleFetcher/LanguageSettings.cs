using System.Collections.Generic;

namespace SubtitleFetcher
{
    public class LanguageSettings
    {
        public LanguageSettings(IEnumerable<string> languages)
        {
            Languages = languages;
        }

        public IEnumerable<string> Languages { get; }
    }
}