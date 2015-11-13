using System.Collections.Generic;
using System.IO;
using System.Linq;
using PortedSubtitleDownloaders.Legacy;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Enhancement.Tvdb;
using SubtitleFetcher.Common.Languages;
using SearchQuery = SubtitleFetcher.Common.SearchQuery;
using Subtitle = SubtitleFetcher.Common.Subtitle;

namespace PortedSubtitleDownloaders.S4U
{
    public class S4UDownloader : ISubtitleDownloader
    {
        private readonly S4UDownloaderImpl _downloader;
        private static readonly Language SwedishLanguage = KnownLanguages.GetLanguageByName("Swedish");

        public S4UDownloader(IApplicationSettings settings)
        {
            var apiKey = settings.GetSetting("S4UApiKey");
            _downloader = new S4UDownloaderImpl(apiKey);
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            var legacySubtitle = new Legacy.Subtitle(subtitle.Id, subtitle.FileName, subtitle.Language.TwoLetterIsoName);
            return _downloader.SaveSubtitle(legacySubtitle);
        }

        public string GetName()
        {
            return _downloader.GetName();
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var tvDbId = GetTvDbId(query);
            var episodeSearchQuery = new EpisodeSearchQuery(query.SeriesTitle, query.Season, query.Episode, tvDbId) { LanguageCodes = new [] { SwedishLanguage.ThreeLetterIsoName } };
            var results = _downloader.SearchSubtitles(episodeSearchQuery);
            return results.Select(r => new Subtitle(r.Id, r.FileName, SwedishLanguage)
            {
                SeriesName = query.SeriesTitle,
                Season = query.Season,
                Episode = query.Episode,
                EndEpisode = query.Episode,
                ReleaseGroup = query.ReleaseGroup
            });
        }

        private static int? GetTvDbId(SearchQuery query)
        {
            var tvDbEnhancement = query.Enhancements.OfType<TvDbEnhancement>().First();
            return tvDbEnhancement?.TvDbId;
        }

        public IEnumerable<Language> SupportedLanguages
        {
            get
            {
                yield return SwedishLanguage;
            }
        }

        public IEnumerable<IEnhancementRequest> EnhancementRequests
        {
            get { yield return new EnhancementRequest<TvDbEnhancement>(); }
        }
    }
}
