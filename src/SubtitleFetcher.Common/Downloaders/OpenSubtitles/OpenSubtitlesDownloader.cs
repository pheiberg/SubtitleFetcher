using System.Collections.Generic;
using System.IO;
using System.Linq;
using CookComputing.XmlRpc;
using SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles
{
    public class OpenSubtitlesDownloader : ISubtitleDownloader
    {
        private readonly OpenSubtitlesApi _api;

        public OpenSubtitlesDownloader(IApplicationSettings applicationSettings)
        {
            var settings = new OpenSubtitlesSettings
            {
                Language = "en",
                Username = "",
                Password = "",
                Url = "http://api.opensubtitles.org/xml-rpc",
                UserAgent = applicationSettings.GetSetting("OpenSubtitlesKey"),
                Timeout = 60000
            };

            var proxy = CreateProxy(settings);
            _api = new OpenSubtitlesApi(settings, proxy);
        }

        private static IOpenSubtitlesProxy CreateProxy(OpenSubtitlesSettings settings)
        {
            var proxy = XmlRpcProxyGen.Create<IOpenSubtitlesProxy>();
            proxy.Timeout = settings.Timeout;
            proxy.Url = settings.Url;
            return proxy;
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            yield break;
        }

        public string GetName()
        {
            return "OpenSubtitles";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var token = _api.Login();
            var languages = query.Languages.Select(l => l.TwoLetterIsoName).ToArray();
            var result = _api.SearchSubtitlesFromQuery(token, languages, query.SerieTitle, query.Season, query.Episode).ToArray();
            return result.Where(s => languages.Contains(s.ISO639)).Select(r => new Subtitle(r.IDSubtitleFile, r.MovieName, BuildFileName(r), KnownLanguages.GetLanguageByTwoLetterIso(r.ISO639)));
        }

        private string BuildFileName(OpenSubtitle subtitle)
        {
            return subtitle.MovieReleaseName;
            var season = subtitle.SeriesSeason.ToString("00");
            var episode = subtitle.SeriesEpisode.ToString("00");
            return $"{subtitle.MovieName}.S{season}.E{episode}.DUMMY";
        }

        public IEnumerable<Language> SupportedLanguages => KnownLanguages.AllLanguages;

        public IEnumerable<IEnhancementRequest> EnhancementRequests
        {
            get{ yield return new EnhancementRequest<OpenSubtitlesFileHashEnhancement>(); }
        }
    }
}