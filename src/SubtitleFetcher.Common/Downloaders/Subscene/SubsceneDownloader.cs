using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher.Common.Downloaders.Subscene
{
    public class SubsceneDownloader : ISubtitleDownloader
    {
        private readonly IEpisodeParser _releaseParser;
        const string SearchUrl = "http://subscene.com/subtitles/release?q=";

        public SubsceneDownloader(IEpisodeParser releaseParser)
        {
            _releaseParser = releaseParser;
        }

        public string GetName()
        {
            return "Subscene";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            var languages = BuildLanguageString(query);
            var queryString = BuildQueryString(query);
            var document = GetSearchPage(languages, queryString);
            var subtitles = GetSubtitlesFromPage(document);

            return subtitles.Select(CreateSubtitle);
        }

        private static string BuildLanguageString(SearchQuery query)
        {
            var languageIds = query.Languages.Select(l => LanguageMappings.Map[l]).ToArray();
            var languages = string.Join(",", languageIds);
            return languages;
        }

        private static string BuildQueryString(SearchQuery query)
        {
            var encodedSeriesName = HttpUtility.UrlEncode(query.SeriesTitle);
            return $"{encodedSeriesName}+s{query.Season.ToString("00")}e{query.Episode.ToString("00")}";
        }

        private static HtmlDocument GetSearchPage(string languages, string queryString)
        {
            var document = new HtmlWeb
            {
                UseCookies = true,
                PreRequest = request =>
                {
                    request.Timeout = 60000;
                    request.CookieContainer.Add(new Cookie("LanguageFilter", languages, "/", ".subscene.com"));
                    return true;
                }
            }.Load(SearchUrl + queryString);
            return document;
        }

        private IEnumerable<SubsceneSubtitle> GetSubtitlesFromPage(HtmlDocument document)
        {
            var anchors = document.DocumentNode.SelectNodes("//table/tbody/tr/td/a");
            
            var subtitles = from anchor in anchors
                            let link = anchor.Attributes["href"].Value
                            let spans = anchor.Elements("span")
                            let language = spans.First().InnerText
                            let releaseName = spans.Last().InnerText
                            let relaseIdentity = _releaseParser.ParseEpisodeInfo(releaseName)
                            select new SubsceneSubtitle
                            {
                                SubtitleLink = link,
                                LanguageName = language,
                                ReleaseName = releaseName,
                                SeriesName = relaseIdentity.SeriesName,
                                Season = relaseIdentity.Season,
                                Episode = relaseIdentity.Episode,
                                EndEpisode = relaseIdentity.EndEpisode,
                                ReleaseGroup = relaseIdentity.ReleaseGroup
                            };
            return subtitles.ToArray();
        }

        private static Subtitle CreateSubtitle(SubsceneSubtitle subtitle)
        {
            return new Subtitle(
                subtitle.SubtitleLink,
                subtitle.ReleaseName,
                KnownLanguages.GetLanguageByName(subtitle.LanguageName))
            {
                SeriesName = subtitle.SeriesName,
                Season = subtitle.Season,
                Episode = subtitle.Episode,
                EndEpisode = subtitle.EndEpisode,
                ReleaseGroup = subtitle.ReleaseGroup
            };
        }

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<Language> SupportedLanguages => LanguageMappings.Map.Keys;

        public IEnumerable<IEnhancementRequest> EnhancementRequests => Enumerable.Empty<IEnhancementRequest>();
    }

    public class SubsceneSubtitle
    {
        public string SubtitleLink { get; set; }
        public string LanguageName { get; set; }
        public string SeriesName { get; set; }
        public int? Season { get; set; }
        public int? Episode { get; set; }
        public int? EndEpisode { get; set; }
        public string ReleaseGroup { get; set; }
        public string ReleaseName { get; set; }
    }
}
