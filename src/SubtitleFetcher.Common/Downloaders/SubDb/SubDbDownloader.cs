using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common.Downloaders.SubDb.Enhancement;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.SubDb
{
    public class SubDbDownloader : ISubtitleDownloader
    {
        private readonly SubDbApi _api;

        public SubDbDownloader()
        {
            _api = new SubDbApi();
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            var languages = new[] { subtitle.Language };
            var download = _api.DownloadSubtitle(subtitle.Id, languages.Select(l => l.TwoLetterIsoName));
            return new []{ download };
        }
        
        public string GetName()
        {
            return "SubDb";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var fileHash = GetFileHash(query);
            var languageCodes = _api.Search(fileHash);
            
            var languages = languageCodes.Select(KnownLanguages.GetLanguageByTwoLetterIso);
            var availableLanguages = GetAvailableLanguagesMatchingSearchQuery(query, languages);
            return availableLanguages.Select(language => CreateSubtitle(fileHash, language, query));
        }

        private static Subtitle CreateSubtitle(string fileHash, Language language, SearchQuery query)
        {
            return new Subtitle(fileHash, "", language)
            {
                SeriesName = query.SeriesTitle,
                Season = query.Season,
                Episode = query.Episode,
                EndEpisode = query.Episode,
                ReleaseGroup = query.ReleaseGroup
            };
        }

        private static string GetFileHash(SearchQuery query)
        {
            var hashEnhancement = query.Enhancements.OfType<SubDbFileHashEnhancement>().SingleOrDefault();
            return hashEnhancement?.FileHash;
        }

        private static IEnumerable<Language> GetAvailableLanguagesMatchingSearchQuery(SearchQuery query, IEnumerable<Language> languages)
        {
            return languages.Intersect(query.Languages);
        }
        
        public IEnumerable<Language> SupportedLanguages {
            get
            {
                yield return KnownLanguages.GetLanguageByName("English");
                yield return KnownLanguages.GetLanguageByName("Spanish");
                yield return KnownLanguages.GetLanguageByName("French");
                yield return KnownLanguages.GetLanguageByName("Italian");
                yield return KnownLanguages.GetLanguageByName("Dutch");
                yield return KnownLanguages.GetLanguageByName("Polish");
                yield return KnownLanguages.GetLanguageByName("Romanian");
                yield return KnownLanguages.GetLanguageByName("Swedish");
                yield return KnownLanguages.GetLanguageByName("Turkish");
            }
        }

        public IEnumerable<IEnhancementRequest> EnhancementRequests
        {
            get
            {
                yield return new EnhancementRequest<SubDbFileHashEnhancement>();
            }
        }
    }
}