using System.Collections.Generic;
using System.IO;
using System.Linq;
using PortedSubtitleDownloaders.Legacy;
using SubtitleFetcher.Common;
using SearchQuery = SubtitleFetcher.Common.SearchQuery;
using Subtitle = SubtitleFetcher.Common.Subtitle;

namespace PortedSubtitleDownloaders.S4U
{
    public class S4UDownloader : ISubtitleDownloader
    {
        readonly S4UDownloaderImpl _downloader;

        public S4UDownloader(IApplicationSettings settings)
        {
            var apiKey = settings.GetSetting("S4UApiKey");
            _downloader = new S4UDownloaderImpl(apiKey);
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            var legacySubtitle = new Legacy.Subtitle(subtitle.Id, subtitle.Title, subtitle.FileName, subtitle.LanguageCode);
            return _downloader.SaveSubtitle(legacySubtitle);
        }

        public string GetName()
        {
            return _downloader.GetName();
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var episodeSearchQuery = new EpisodeSearchQuery(query.SerieTitle, query.Season, query.Episode) { LanguageCodes = query.LanguageCodes };
            var results = _downloader.SearchSubtitles(episodeSearchQuery);
            return results.Select(r => new Subtitle(r.Id, r.Title, r.FileName, r.LanguageCode));
        }

        public IEnumerable<string> LanguageLimitations => new[] { "swe" };
    }
}
