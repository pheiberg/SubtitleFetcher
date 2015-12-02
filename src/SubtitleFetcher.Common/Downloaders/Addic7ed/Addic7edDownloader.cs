using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class Addic7edDownloader : ISubtitleDownloader
    {
        const string BaseUrl = "http://www.addic7ed.com";
        const string UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86";
        const string Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        const string AcceptEncoding = "gzip, deflate, sdch";
        const string AcceptCharset = "UTF-8";


        public string GetName() => "Addic7ed";

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var seriesTitle = HttpUtility.UrlEncode(query.SeriesTitle.Replace(":", " "));
            var url = $"{BaseUrl}/search.php?search={seriesTitle}+{query.Season}x{query.Episode}&Submit=Search";
            var web = CreateHtmlWeb();
            var document = web.Load(url);
            throw new NotImplementedException();
        }

        private static HtmlWeb CreateHtmlWeb()
        {
            return new HtmlWeb
            {
                PreRequest = request =>
                {
                    request.AllowAutoRedirect = true;
                    request.MaximumAutomaticRedirections = 5;
                    request.Timeout = 60000;
                    request.UserAgent = UserAgent;
                    request.Accept = Accept;
                    return true;
                }
            };
        }

        public IEnumerable<Language> SupportedLanguages => new[]
        {
            KnownLanguages.GetLanguageByName("Arabic"),
            KnownLanguages.GetLanguageByName("English"),
            KnownLanguages.GetLanguageByName("French"),
            KnownLanguages.GetLanguageByName("German"),
            KnownLanguages.GetLanguageByName("Hungarian"),
            KnownLanguages.GetLanguageByName("Italian"),
            KnownLanguages.GetLanguageByName("Persian"),
            KnownLanguages.GetLanguageByName("Polish"),
            KnownLanguages.GetLanguageByName("Portuguese"),
            KnownLanguages.GetLanguageByName("Romanian"),
            KnownLanguages.GetLanguageByName("Russian"),
            KnownLanguages.GetLanguageByName("Spanish"),
            KnownLanguages.GetLanguageByName("Swedish")
        };

        public IEnumerable<IEnhancementRequest> EnhancementRequests => Enumerable.Empty<IEnhancementRequest>();
    }

}
