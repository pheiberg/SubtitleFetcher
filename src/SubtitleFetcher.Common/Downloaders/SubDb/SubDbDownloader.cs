using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SubtitleFetcher.Common.Downloaders.SubDb
{
    public class SubDbDownloader : ISubtitleDownloader
    {
        private static readonly IDictionary<string, string> LanguageLookup = new Dictionary<string, string>
        {
            {"eng", "en"},
            {"spa", "es"},
            {"fre", "fr"},
            {"ita", "it"},
            {"dut", "nl"},
            {"pol", "pl"},
            {"por", "pt"},
            {"rom", "ro"},
            {"swe", "sv"},
            {"tur", "tr"}
        };

        private readonly SubDbApi _api;

        public SubDbDownloader(SubDbApi api)
        {
            _api = api;
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            var languages = new[] { LanguageLookup[subtitle.LanguageCode] };
            var download = _api.DownloadSubtitle(subtitle.Id, languages);
            return new []{ download };
        }
        
        public string GetName()
        {
            return "SubDb";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var hash = query.FileHash;
            var languages = _api.Search(hash);
            string constructedFileName = $"{query.SerieTitle}.S{query.Season.ToString("00")}.E{query.Episode.ToString("00")}.DUMMY-{query.ReleaseGroup}";

            var availableLanguages = GetAvailableLanguagesMatchingSearchQuery(query, languages);
            return availableLanguages.Select(language => new Subtitle(hash, query.SerieTitle, constructedFileName, language));
        }

        private static IEnumerable<string> GetAvailableLanguagesMatchingSearchQuery(SearchQuery query, IEnumerable<string> languages)
        {
            return languages.Where(l => !string.IsNullOrWhiteSpace(l) 
                             && query.LanguageCodes.Contains(TranslateLanguageCode(l)));
        }

        private static string TranslateLanguageCode(string language)
        {
            return LanguageLookup.SingleOrDefault(l => string.Equals(l.Value, language, StringComparison.OrdinalIgnoreCase)).Key;
        }

        public IEnumerable<string> LanguageLimitations => LanguageLookup.Keys;
    }
}