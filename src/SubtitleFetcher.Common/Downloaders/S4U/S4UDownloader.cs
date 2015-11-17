using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Enhancement.Tvdb;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class S4UDownloader : ISubtitleDownloader
    {
        private readonly S4UApi _api;

        public S4UDownloader(S4USettings settings)
        {
            _api = new S4UApi(settings);
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            throw new System.NotImplementedException();
        }

        public string GetName()
        {
            return "S4U";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var results = _api.SearchByTitle(query.SeriesTitle, new S4ULimits {Season = query.Season, Episode = query.Episode});
            return results.Series.First().Subs.Select(r => new Subtitle(r.download_file, r.file_name, SupportedLanguages.Single()));
        }

        public IEnumerable<Language> SupportedLanguages => new[] { KnownLanguages.GetLanguageByName("Swedish")};
        public IEnumerable<IEnhancementRequest> EnhancementRequests => new[] { new EnhancementRequest<TvDbEnhancement>()  };
    }
}
