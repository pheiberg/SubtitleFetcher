using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        const string SearchUrl = SiteUrl + "/subtitles/release?q=";

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
                let spans = anchor.Elements("span").ToArray()
                let firstSpan = spans.First()
                let ratingClass = firstSpan.Attributes["class"].Value
                let ratingType = ratingClass.Contains("bad-icon") ? -1 : ratingClass.Contains("positive-icon") ? 1 : 0  
                let language = firstSpan.InnerText.Trim()
                let releaseName = spans.Last().InnerText.Trim()
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
                    ReleaseGroup = relaseIdentity.ReleaseGroup,
                    RatingType = ratingType
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
            var detailsDocument = GetDetailsDocument(subtitle.Id);
            var downloadLink = GetDownloadLink(detailsDocument);
            if (downloadLink == null)
                return Enumerable.Empty<FileInfo>();

            return DownloadSubtitle(downloadLink, subtitle.FileName);
        }

        private static HtmlDocument GetDetailsDocument(string link)
        {
            return new HtmlWeb().Load(SiteUrl + link);
        }

        private static string GetDownloadLink(HtmlDocument detailsDocument)
        {
            var downloadLink = detailsDocument.DocumentNode.SelectSingleNode("//a[@id=downloadButton]");
            return downloadLink.Attributes["href"].Value;
        }

        private IEnumerable<FileInfo> DownloadSubtitle(string downloadLink, string fileName)
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

        public IEnumerable<Language> SupportedLanguages => LanguageMappings.Map.Keys;

        public IEnumerable<IEnhancementRequest> EnhancementRequests => Enumerable.Empty<IEnhancementRequest>();
    }
}
