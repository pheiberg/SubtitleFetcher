using System;
using System.Collections.Generic;
using System.Linq;

namespace PortedSubtitleDownloaders.Legacy
{
    public static class Languages
    {
        private static readonly SubLang DefaultLanguage = new SubLang("eng", "English");
        private static readonly SubLang[] Aliases = { new SubLang("nld", "Dutch") };

        private static readonly SubLang[] languages = {
            new SubLang("bos", "Bosnian"),
            new SubLang("slv", "Slovenian"),
            new SubLang("hrv", "Croatian"),
            new SubLang("srp", "Serbian"),
            new SubLang("eng", "English"),
            new SubLang("spa", "Spanish"),
            new SubLang("fre", "French"),
            new SubLang("gre", "Greek"),
            new SubLang("ger", "German"),
            new SubLang("rus", "Russian"),
            new SubLang("chi", "Chinese"),
            new SubLang("por", "Portuguese"),
            new SubLang("dut", "Dutch"),
            new SubLang("ita", "Italian"),
            new SubLang("rum", "Romanian"),
            new SubLang("cze", "Czech"),
            new SubLang("ara", "Arabic"),
            new SubLang("pol", "Polish"),
            new SubLang("tur", "Turkish"),
            new SubLang("swe", "Swedish"),
            new SubLang("fin", "Finnish"),
            new SubLang("hun", "Hungarian"),
            new SubLang("dan", "Danish"),
            new SubLang("heb", "Hebrew"),
            new SubLang("est", "Estonian"),
            new SubLang("slo", "Slovak"),
            new SubLang("ind", "Indonesian"),
            new SubLang("per", "Persian"),
            new SubLang("bul", "Bulgarian"),
            new SubLang("jpn", "Japanese"),
            new SubLang("alb", "Albanian"),
            new SubLang("bel", "Belarusian"),
            new SubLang("hin", "Hindi"),
            new SubLang("gle", "Irish"),
            new SubLang("ice", "Icelandic"),
            new SubLang("cat", "Catalan"),
            new SubLang("kor", "Korean"),
            new SubLang("lav", "Latvian"),
            new SubLang("lit", "Lithuanian"),
            new SubLang("mac", "Macedonian"),
            new SubLang("nor", "Norwegian"),
            new SubLang("tha", "Thai"),
            new SubLang("ukr", "Ukrainian"),
            new SubLang("vie", "Vietnamese")
        };

        public static string GetLanguageCode(string languageName)
        {
            return FindLanguageCode(languageName) ?? DefaultLanguage.Code;
        }

        public static string FindLanguageCode(string languageName)
        {
            if (string.IsNullOrEmpty(languageName))
                throw new ArgumentException("Language name cannot be null or empty!");
            SubLang subLang = languages.FirstOrDefault(l => l.Name.Equals(languageName, StringComparison.OrdinalIgnoreCase));
            return subLang?.Code;
        }

        public static string GetLanguageName(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                throw new ArgumentException("Language code cannot be null or empty!");
            if (languageCode.Length != 3)
                throw new ArgumentException("Invalid ISO 639-2 language code!");
            SubLang languageCodeInternal = FindLanguageByLanguageCodeInternal(languageCode);
            return languageCodeInternal != null ? languageCodeInternal.Name : DefaultLanguage.Name;
        }

        public static bool IsSupportedLanguageCode(string languageCode)
        {
            return FindLanguageByLanguageCodeInternal(languageCode) != null;
        }

        public static bool IsSupportedLanguageName(string languageName)
        {
            return languages.Any(lang => lang.Name.Equals(languageName, StringComparison.OrdinalIgnoreCase));
        }

        public static List<string> GetLanguageNames()
        {
            return languages.Select(lang => lang.Name).ToList();
        }

        private static SubLang FindLanguageByLanguageCodeInternal(string languageCode)
        {
            return languages.FirstOrDefault(l => l.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase)) ?? Aliases.FirstOrDefault(l => l.Code.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
        }
    }
}