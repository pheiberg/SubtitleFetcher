using System.Collections.Generic;

namespace SubtitleFetcher.Common
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