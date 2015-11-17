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

        public S4UDownloader(IApplicationSettings applicationSettings)
        {
            var settings = new S4USettings
            {
                ApiKey = applicationSettings.GetSetting("S4UApiKey")
            };
            _api = new S4UApi(settings);
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string fileName = Path.Combine(Path.GetTempPath(), subtitle.FileName);
            new WebDownloader().DownloadFile(subtitle.Id, fileName);
            return new[] { new FileInfo(fileName) };
        }

        public string GetName()
        {
            return "S4U";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var results = _api.SearchByTitle(query.SeriesTitle, new S4ULimits {Season = query.Season, Episode = query.Episode});
            return results.Series.SelectMany(serie => 
                serie.Subs.Select(sub => 
                    new Subtitle(sub.DownloadFile, sub.FileName + "." + sub.FileType, SupportedLanguages.Single())
                    {
                        SeriesName = serie.Title,
                        Season = sub.Season,
                        Episode = sub.Episode,
                        EndEpisode = sub.Episode,
                        ReleaseGroup = sub.ReleaseGroup
                    }));
        }

        public IEnumerable<Language> SupportedLanguages => new[] { KnownLanguages.GetLanguageByName("Swedish")};
        public IEnumerable<IEnhancementRequest> EnhancementRequests => new[] { new EnhancementRequest<TvDbEnhancement>()  };
    }
}
