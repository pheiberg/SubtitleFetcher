﻿using System.Collections.Generic;
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
            yield return _api.DownloadSubtitle(subtitle.Id, subtitle.FileName);
        }

        public string GetName()
        {
            return "OpenSubtitles";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var token = _api.Login();
            var languages = query.Languages.Select(l => l.TwoLetterIsoName).ToArray();
            var fileHashEnhancement = query.Enhancements.OfType<OpenSubtitlesFileHashEnhancement>().SingleOrDefault();
            var hashResults = SearchByHash(fileHashEnhancement, token, languages);
            return hashResults.Any() ? hashResults : SearchByQuery(query, token, languages);
        }

        private IEnumerable<Subtitle> SearchByHash(OpenSubtitlesFileHashEnhancement fileHashEnhancement, string token, string[] languages)
        {
            if (fileHashEnhancement == null)
                return Enumerable.Empty<Subtitle>();

            var result = _api.SearchSubtitlesFromFile(token, languages, fileHashEnhancement.FileHash, fileHashEnhancement.FileByteSize).ToArray();
            return FilterAndConvertResults(languages, result);
        }

        private IEnumerable<Subtitle> SearchByQuery(SearchQuery query, string token, string[] languages)
        {
            var result = _api.SearchSubtitlesFromQuery(token, languages, query.SerieTitle, query.Season, query.Episode).ToArray();
            return FilterAndConvertResults(languages, result);
        }

        private static IEnumerable<Subtitle> FilterAndConvertResults(string[] languages, OpenSubtitle[] result)
        {
            var validResults = FilterOutUnwantedHits(result, languages);
            return validResults.Select(r => new Subtitle(r.SubDownloadLink, r.MovieReleaseName + "." + r.SubFormat, KnownLanguages.GetLanguageByTwoLetterIso(r.ISO639)));
        }

        private static IEnumerable<OpenSubtitle> FilterOutUnwantedHits(OpenSubtitle[] results, IEnumerable<string> languages)
        {
            return results.Where(subtitle => languages.Contains(subtitle.ISO639) 
                           && subtitle.MovieKind == OpenSubtitlesKind.Episode);
        }

        public IEnumerable<Language> SupportedLanguages => KnownLanguages.AllLanguages;

        public IEnumerable<IEnhancementRequest> EnhancementRequests
        {
            get{ yield return new EnhancementRequest<OpenSubtitlesFileHashEnhancement>(); }
        }
    }
}