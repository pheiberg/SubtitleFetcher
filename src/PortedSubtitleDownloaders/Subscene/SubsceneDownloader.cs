using System.Collections.Generic;
using System.IO;
using System.Linq;
using PortedSubtitleDownloaders.Legacy;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Download;
using Subtitle = PortedSubtitleDownloaders.Legacy.Subtitle;

namespace PortedSubtitleDownloaders.Subscene
{
    public class SubsceneDownloader : ISubtitleDownloader
    {
        readonly SubsceneDownloaderImpl _downloaderImpl = new SubsceneDownloaderImpl();

        public string GetName()
        {
            return _downloaderImpl.GetName();
        }

        public IEnumerable<SubtitleFetcher.Common.Subtitle> SearchSubtitles(SubtitleFetcher.Common.SearchQuery query)
        {
            var episodeSearchQuery = new EpisodeSearchQuery(query.SerieTitle, query.Season, query.Episode) { LanguageCodes = query.LanguageCodes };
            var results = _downloaderImpl.SearchSubtitles(episodeSearchQuery);
            return results.Select(r => new SubtitleFetcher.Common.Subtitle(r.Id, r.Title, r.FileName, r.LanguageCode));
        }

        public IEnumerable<FileInfo> SaveSubtitle(SubtitleFetcher.Common.Subtitle subtitle)
        {
            var localSubtitle = new Subtitle(subtitle.Id, subtitle.Title, subtitle.FileName, subtitle.LanguageCode);
            return _downloaderImpl.SaveSubtitle(localSubtitle);
        }

        public IEnumerable<string> LanguageLimitations => Enumerable.Empty<string>();
    }
}
