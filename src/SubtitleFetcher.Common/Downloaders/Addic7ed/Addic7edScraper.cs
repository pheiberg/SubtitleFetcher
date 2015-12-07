using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class Addic7edScraper
    {
        const string BaseUrl = "http://www.addic7ed.com";
        const string UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86";
        const string Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        const string AcceptEncoding = "gzip, deflate, sdch";
        const string AcceptCharset = "UTF-8";
       
        public int? FindSeries(string seriesName)
        {
            var shows = GetShows();

            var series = shows.SingleOrDefault(s => 
                string.Equals(s.Name, seriesName, StringComparison.OrdinalIgnoreCase));

            return series?.Id;
        }

        public IEnumerable<Addic7edShow> GetShows()
        {
            HtmlNode.ElementsFlags.Remove("option");
            string url = $"{BaseUrl}/shows.php";
            var web = CreateHtmlWeb();
            var document = web.Load(url);
            var shows = document.DocumentNode.SelectNodes("//select[@id='qsShow']/option").ToArray();
            var result = shows.Where(s => s.InnerText.Trim().Length > 0 && s.Attributes["value"] != null)
                .Select(s => new Addic7edShow
                {
                    Id = int.Parse(s.Attributes["value"].Value.Trim()),
                    Name = s.InnerText.Trim()
                });
            return result;
        }

        public IEnumerable<Subtitle> SearchSubtitles(int seriesId, string seriesTitle, int season, int episode, IEnumerable<Language> languages)
        {
            var url = $"{BaseUrl}/ajax_loadShow.php?show={seriesId}&season={season}&langs=|1|8|11|7|9|10|26|&hd=0&hi=0";
            var web = CreateHtmlWeb();
            var document = web.Load(url);
            var subtitleContainers = document.DocumentNode.SelectNodes("//tr[contains(@class, 'completed')]");
            var addic7EdSubtitles = subtitleContainers.Select(sc => new Addic7edSubtitle
            {
                Season = int.Parse(sc.SelectSingleNode("./td[1]").InnerText.Trim()),
                Episode = int.Parse(sc.SelectSingleNode("./td[2]").InnerText.Trim()),
                Version = sc.SelectSingleNode("./td[5]").InnerText.Trim(),
                DowloadLink = sc.SelectSingleNode(".//a[text()='Download']").Attributes["href"].Value.Trim(),
                Language = sc.SelectSingleNode("./td[4]").InnerText.Trim(),
                HearingImpaired = sc.SelectSingleNode("./td[5]").InnerText.Trim() == "✔",
                Corrected = sc.SelectSingleNode("./td[6]").InnerText.Trim() == "✔",
                HighDefinition = sc.SelectSingleNode("./td[7]").InnerText.Trim() == "✔"
            });

            var subtitles = addic7EdSubtitles.Select(s =>
                new Subtitle(s.DowloadLink,
                    $"{seriesTitle}.S{s.Season.ToString("00")}.E{s.Episode.ToString("00")}.DUMMY-{s.Version}",
                    ParseLanguage(s.Language))
                {
                    SeriesName = seriesTitle,
                    Season = s.Season,
                    Episode = s.Episode,
                    EndEpisode = s.Episode,
                    ReleaseGroup = s.Version
                });
            return subtitles
                .Where(s => languages.Contains(s.Language) 
                && s.Season == season
                && s.Episode == episode);
        }

        private static Language ParseLanguage(string language)
        {
            return KnownLanguages.AllLanguages.SingleOrDefault(l => string.Equals(l.Name, language, StringComparison.OrdinalIgnoreCase));
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
    }
    public class Addic7edShow
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
