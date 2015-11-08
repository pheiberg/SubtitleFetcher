using System.Collections.Generic;

namespace SubtitleFetcher.Common.Languages
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