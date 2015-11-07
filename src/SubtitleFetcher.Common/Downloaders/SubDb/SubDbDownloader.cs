using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common.Downloaders.SubDb.Enhancement;
using SubtitleFetcher.Common.Enhancement;

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

        public SubDbDownloader()
        {
            _api = new SubDbApi();
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
            var fileHash = GetFileHash(query);
            var languages = _api.Search(fileHash);
            string constructedFileName = $"{query.SerieTitle}.S{query.Season.ToString("00")}.E{query.Episode.ToString("00")}.DUMMY-{query.ReleaseGroup}";

            var availableLanguages = GetAvailableLanguagesMatchingSearchQuery(query, languages);
            return availableLanguages.Select(language => new Subtitle(fileHash, query.SerieTitle, constructedFileName, language));
        }

        private static string GetFileHash(SearchQuery query)
        {
            var hashEnhancement = query.Enhancements.OfType<FileHashEnhancement>().SingleOrDefault();
            return hashEnhancement?.FileHash;
        }

        private static IEnumerable<string> GetAvailableLanguagesMatchingSearchQuery(SearchQuery query, IEnumerable<string> languages)
        {
            var mappedLanguages = languages.Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(TranslateLanguageCode);
            return mappedLanguages.Where(l => query.LanguageCodes.Contains(l));
        }

        private static string TranslateLanguageCode(string language)
        {
            return LanguageLookup.SingleOrDefault(l => string.Equals(l.Value, language, StringComparison.OrdinalIgnoreCase)).Key;
        }

        public IEnumerable<string> LanguageLimitations => LanguageLookup.Keys;

        public IEnumerable<IEnhancementRequest> EnhancementRequests
        {
            get
            {
                yield return new EnhancementRequest<FileHashEnhancement>();
            }
        }
    }
}