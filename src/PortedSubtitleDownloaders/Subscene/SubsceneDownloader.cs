using System.Collections.Generic;
using System.IO;
using System.Linq;
using PortedSubtitleDownloaders.Legacy;
using SubtitleFetcher.Common.Downloaders;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;
using Subtitle = SubtitleFetcher.Common.Subtitle;

namespace PortedSubtitleDownloaders.Subscene
{
    public class SubsceneDownloader : ISubtitleDownloader
    {
        readonly SubsceneDownloaderImpl _downloaderImpl = new SubsceneDownloaderImpl();

        public string GetName()
        {
            return _downloaderImpl.GetName();
        }

        public IEnumerable<Subtitle> SearchSubtitles(SubtitleFetcher.Common.SearchQuery query)
        {
            var languageCodes = query.Languages.Select(l => l.ThreeLetterIsoName).ToArray();
            var episodeSearchQuery = new EpisodeSearchQuery(query.SeriesTitle, query.Season, query.Episode) { LanguageCodes = languageCodes };
            var results = _downloaderImpl.SearchSubtitles(episodeSearchQuery);
            return results.Select(r => new Subtitle(r.Id, r.FileName, KnownLanguages.GetLanguageByThreeLetterIso(r.LanguageCode))
            {
                SeriesName = query.SeriesTitle,
                Season = query.Season,
                Episode = query.Episode,
                EndEpisode = query.Episode,
                ReleaseGroup = query.ReleaseGroup
            });
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            var localSubtitle = new Legacy.Subtitle(subtitle.Id, subtitle.FileName, subtitle.Language.ThreeLetterIsoName);
            return _downloaderImpl.SaveSubtitle(localSubtitle);
        }
        
        public IEnumerable<Language> SupportedLanguages => KnownLanguages.AllLanguages;

        public IEnumerable<IEnhancementRequest> EnhancementRequests => Enumerable.Empty<IEnhancementRequest>();
    }
}
