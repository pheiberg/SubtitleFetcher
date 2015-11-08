using System.Collections.Generic;

namespace SubtitleFetcher.Common
{
    public class LanguageSettings
    {
        public LanguageSettings(IEnumerable<Language> languages)
        {
            Languages = languages;
        }

        public IEnumerable<Language> Languages { get; }
    }
}