using System;
using System.Linq;

namespace PortedSubtitleDownloaders.Legacy
{
    public abstract class SubtitleSearchQuery
    {
        private string[] _languageCodes;

        public string[] LanguageCodes
        {
            get
            {
                return _languageCodes;
            }
            set
            {
                if (value.Any(lang => lang.Length != 3))
                    throw new ArgumentException("Language codes must be ISO 639-2 Code!");
                _languageCodes = value;
            }
        }

        protected SubtitleSearchQuery()
        {
            LanguageCodes = new[] { "eng" };
        }

        public bool HasLanguageCode(string languageCode)
        {
            if (languageCode == null)
                return false;
            if (languageCode.Length != 3)
                throw new ArgumentException("Language code must be ISO 639-2 Code!");
            return _languageCodes.Any(code => code.Equals(languageCode.ToLower()));
        }
    }
}