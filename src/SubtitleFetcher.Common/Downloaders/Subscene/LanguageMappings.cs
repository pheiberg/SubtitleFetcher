using System.Collections.Generic;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.Subscene
{
    public class LanguageMappings
    {
        public static readonly IReadOnlyDictionary<Language, int> Map = new Dictionary<Language, int>
        {
            { KnownLanguages.GetLanguageByName("Albanian"), 1 },
            { KnownLanguages.GetLanguageByName("Arabic"), 2 },
            { KnownLanguages.GetLanguageByName("Bulgarian"), 5 },
            { KnownLanguages.GetLanguageByName("Catalan"), 49 },
            { KnownLanguages.GetLanguageByName("Chinese"), 7 },
            { KnownLanguages.GetLanguageByName("Croatian"), 8 },
            { KnownLanguages.GetLanguageByName("Czech"), 9 },
            { KnownLanguages.GetLanguageByName("Danish"), 10 },
            { KnownLanguages.GetLanguageByName("Dutch"), 11 },
            { KnownLanguages.GetLanguageByName("English"), 13 },
            { KnownLanguages.GetLanguageByName("Estonian"), 16 },
            { KnownLanguages.GetLanguageByName("Persian"), 46 },
            { KnownLanguages.GetLanguageByName("Finnish"), 17 },
            { KnownLanguages.GetLanguageByName("French"), 18 },
            { KnownLanguages.GetLanguageByName("German"), 19 },
            { KnownLanguages.GetLanguageByName("Greek"), 21 },
            { KnownLanguages.GetLanguageByName("Hebrew"), 22 },
            { KnownLanguages.GetLanguageByName("Hindi"), 51 },
            { KnownLanguages.GetLanguageByName("Hungarian"), 23 },
            { KnownLanguages.GetLanguageByName("Icelandic"), 25 },
            { KnownLanguages.GetLanguageByName("Indonesian"), 44 },
            { KnownLanguages.GetLanguageByName("Italian"), 26 },
            { KnownLanguages.GetLanguageByName("Japanese"), 27 },
            { KnownLanguages.GetLanguageByName("Korean"), 28 },
            { KnownLanguages.GetLanguageByName("Latvian"), 29 },
            { KnownLanguages.GetLanguageByName("Lithuanian"), 43 },
            { KnownLanguages.GetLanguageByName("Macedonian"), 48 },
            { KnownLanguages.GetLanguageByName("Norwegian"), 30 },
            { KnownLanguages.GetLanguageByName("Polish"), 31 },
            { KnownLanguages.GetLanguageByName("Portuguese"), 32 },
            { KnownLanguages.GetLanguageByName("Romanian"), 33 },
            { KnownLanguages.GetLanguageByName("Russian"), 34 },
            { KnownLanguages.GetLanguageByName("Serbian"), 35 },
            { KnownLanguages.GetLanguageByName("Slovak"), 36 },
            { KnownLanguages.GetLanguageByName("Slovenian"), 37 },
            { KnownLanguages.GetLanguageByName("Spanish"), 38 },
            { KnownLanguages.GetLanguageByName("Swedish"), 39 },
            { KnownLanguages.GetLanguageByName("Thai"), 40 },
            { KnownLanguages.GetLanguageByName("Turkish"), 41 },
            { KnownLanguages.GetLanguageByName("Vietnamese"), 45 }
        };
    }
}
