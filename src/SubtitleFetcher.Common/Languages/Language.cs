using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitleFetcher.Common.Languages
{
    public class Language
    {
        public string Name { get; }
        public string TwoLetterIsoName { get; }
        public string ThreeLetterIsoName { get; }

        public Language(string name, string twoLetterIsoName, string threeLetterIsoName)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (twoLetterIsoName == null) throw new ArgumentNullException(nameof(twoLetterIsoName));
            if (threeLetterIsoName == null) throw new ArgumentNullException(nameof(threeLetterIsoName));
            Name = name;
            TwoLetterIsoName = twoLetterIsoName;
            ThreeLetterIsoName = threeLetterIsoName;
        }

        protected bool Equals(Language other)
        {
            return string.Equals(Name, other.Name) 
                && string.Equals(TwoLetterIsoName, other.TwoLetterIsoName) 
                && string.Equals(ThreeLetterIsoName, other.ThreeLetterIsoName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Language) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name.GetHashCode();
                hashCode = (hashCode*397) ^ TwoLetterIsoName.GetHashCode();
                hashCode = (hashCode*397) ^ ThreeLetterIsoName.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Language left, Language right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Language left, Language right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public static class KnownLanguages
    {
        public static readonly IEnumerable<Language> AllLanguages = (new[]{
            new Language("Arabic", "ar", "ara"),
            new Language("Belarusian", "be", "bel"),
            new Language("Bulgarian", "bg", "bul"),
            new Language("Bosnian", "bs", "bos"),
            new Language("Catalan", "ca", "cat"),
            new Language("Czech", "cs", "ces"),
            new Language("Welsh", "cy", "cym"),
            new Language("Danish", "da", "dan"),
            new Language("German", "de", "deu"),
            new Language("Greek", "el", "ell"),
            new Language("English", "en", "eng"),
            new Language("Spanish", "es", "spa"),
            new Language("Estonian", "et", "est"),
            new Language("Persian", "fa", "fas"),
            new Language("Finnish", "fi", "fin"),
            new Language("French", "fr", "fra"),
            new Language("Irish", "ga", "gle"),
            new Language("Hebrew", "he", "heb"),
            new Language("Hindi", "hi", "hin"),
            new Language("Croatian", "hr", "hrv"),
            new Language("Hungarian", "hu", "hun"),
            new Language("Armenian", "hy", "hye"),
            new Language("Indonesian", "id", "ind"),
            new Language("Icelandic", "is", "isl"),
            new Language("Italian", "it", "ita"),
            new Language("Japanese", "ja", "jpn"),
            new Language("Korean", "ko", "kor"),
            new Language("Lithuanian", "lt", "lit"),
            new Language("Latvian", "lv", "lav"),
            new Language("Macedonian", "mk", "mkd"),
            new Language("Dutch", "nl", "nld"),
            new Language("Norwegian", "nb", "nob"),
            new Language("Polish", "pl", "pol"),
            new Language("Portuguese", "pt", "por"),
            new Language("Romanian", "ro", "ron"),
            new Language("Russian", "ru", "rus"),
            new Language("Slovak", "sk", "slk"),
            new Language("Slovenian", "sl", "slv"),
            new Language("Albanian", "sq", "sqi"),
            new Language("Serbian", "sr", "srp"),
            new Language("Swedish", "sv", "swe"),
            new Language("Thai", "th", "tha"),
            new Language("Turkish", "tr", "tur"),
            new Language("Vietnamese", "vi", "vie"),
            new Language("Chinese", "zh", "zho")
        });

        public static Language GetLanguageByName(string name)
        {
            return AllLanguages.SingleOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static Language GetLanguageByTwoLetterIso(string code)
        {
            return AllLanguages.SingleOrDefault(l => l.TwoLetterIsoName.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public static Language GetLanguageByThreeLetterIso(string code)
        {
            return AllLanguages.SingleOrDefault(l => l.ThreeLetterIsoName.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
    }
}
