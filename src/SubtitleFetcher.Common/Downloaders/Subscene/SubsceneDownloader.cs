using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using SharpCompress.Reader;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher.Common.Downloaders.Subscene
{
    public class SubsceneDownloader : ISubtitleDownloader
    {
        private readonly IEpisodeParser _releaseParser;
        const string SiteUrl = "http://subscene.com";
        const string SearchUrl = "http://v2.subscene.com/s.aspx?q=";

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
            subtitles = FilterOutInvalidEpisodes(subtitles, query);
            var result = ConvertToGlobalSubtitles(subtitles);
            
            return FilterOutLanguageNotInQuery(query, result);
        }

        private static string BuildLanguageString(SearchQuery query)
        {
            var languageIds = query.Languages.Select(l => LanguageMappings.Map[l]).ToArray();
            return string.Join("-", languageIds);
        }

        private static string BuildQueryString(SearchQuery query)
        {
            var encodedSeriesName = HttpUtility.UrlEncode(query.SeriesTitle);
            return $"{encodedSeriesName}+s{query.Season.ToString("00")}e{query.Episode.ToString("00")}";
        }

        private static HtmlDocument GetSearchPage(string languages, string queryString)
        {
            var htmlWeb = CreateHtmlWeb(languages);
            var document = htmlWeb.Load(SearchUrl + queryString);
            return document;
        }

        private static HtmlWeb CreateHtmlWeb(string languages = null)
        {
            return new HtmlWeb
            {
                UseCookies = true,
                PreRequest = request =>
                {
                    request.AllowAutoRedirect = true;
                    request.MaximumAutomaticRedirections = 5;
                    request.Timeout = 60000;
                    if(languages != null)
                    {
                        request.CookieContainer.Add(new Cookie("subscene_sLanguageIds", languages, "/", ".subscene.com"));
                    }
                    return true;
                }
            };
        }

        private IEnumerable<SubsceneSubtitle> GetSubtitlesFromPage(HtmlDocument document)
        {
            var anchors = document.DocumentNode.SelectNodes("//*/table/tr/td/a");

            var subtitles = from anchor in anchors
                            let link = anchor.Attributes["href"].Value
                            where link.Contains("subtitle-") && link.EndsWith(".aspx")
                            select CreateSubtitleFromLink(anchor, link);
            return subtitles.ToArray();
        }

        private SubsceneSubtitle CreateSubtitleFromLink(HtmlNode anchor, string link)
        {
            const string goodRatingClassName = "r100";
            const string neutralRatingClassName = "r0";
            var spans = anchor.Elements("span").ToArray();
            var firstSpan = spans.First();
            var ratingClass = firstSpan.Attributes["class"].Value;
            var ratingType = ratingClass.Contains(goodRatingClassName) ? 1 : ratingClass.Contains(neutralRatingClassName) ? 0 : -1;
            var language = firstSpan.InnerText.Trim();
            var lastSpan = spans.Last();
            var releaseName = RemoveComments(lastSpan?.InnerText.Trim());
            var relaseIdentity = _releaseParser.ParseEpisodeInfo(releaseName);
            var subtitle = new SubsceneSubtitle
            {
                SubtitleLink = link,
                LanguageName = language,
                ReleaseName = releaseName,
                SeriesName = relaseIdentity.SeriesName,
                Season = relaseIdentity.Season,
                Episode = relaseIdentity.Episode,
                EndEpisode = relaseIdentity.EndEpisode,
                ReleaseGroup = relaseIdentity.ReleaseGroup,
                RatingType = ratingType
            };
            return subtitle;
        }

        private static IEnumerable<SubsceneSubtitle> FilterOutInvalidEpisodes(IEnumerable<SubsceneSubtitle> subtitles, SearchQuery query)
        {
            return subtitles.Where(s => s.Season == query.Season && s.Episode == query.Episode);
        }

        private static string RemoveComments(string releaseText)
        {
            return Regex.Replace(releaseText, @"\(.*?\)", "").Trim();
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
            var detailsDocument = GetDetailsDocument(subtitle.Id);
            var downloadLink = GetDownloadLink(detailsDocument);
            if (downloadLink == null)
                return Enumerable.Empty<FileInfo>();

            return DownloadSubtitle(downloadLink, subtitle.FileName);
        }

        private static HtmlDocument GetDetailsDocument(string link)
        {
            return CreateHtmlWeb().Load(SiteUrl + link);
        }

        private static string GetDownloadLink(HtmlDocument detailsDocument)
        {
            var downloadLink = detailsDocument.DocumentNode.SelectSingleNode("//a[@id='downloadButton']");
            return downloadLink.Attributes["href"].Value;
        }

        private static IEnumerable<FileInfo> DownloadSubtitle(string downloadLink, string fileName)
        {
            string address = SiteUrl + downloadLink;
            string subtitleFilePath = Path.Combine(Path.GetTempPath(), fileName);
            var data = new WebDownloader().DownloadData(address);
            using (var fileStream = new MemoryStream(data))
            {
                UnzipSubtitleToFile(fileStream, subtitleFilePath);
            }

            return new[] { new FileInfo(subtitleFilePath) };
        }

        private static void UnzipSubtitleToFile(Stream zipFile, string subFileName)
        {
            using (var reader = ReaderFactory.Open(zipFile))
            {
                reader.MoveToNextEntry();
                reader.WriteEntryTo(subFileName);
            }
        }

        private static IEnumerable<Subtitle> ConvertToGlobalSubtitles(IEnumerable<SubsceneSubtitle> subtitles)
        {
            return subtitles.Select(CreateSubtitle);
        }

        private static IEnumerable<Subtitle> FilterOutLanguageNotInQuery(SearchQuery query, IEnumerable<Subtitle> result)
        {
            return result.Where(s => query.Languages.Contains(s.Language));
        }

        public IEnumerable<Language> SupportedLanguages => LanguageMappings.Map.Keys;

        public IEnumerable<IEnhancementRequest> EnhancementRequests => Enumerable.Empty<IEnhancementRequest>();
    }
}
