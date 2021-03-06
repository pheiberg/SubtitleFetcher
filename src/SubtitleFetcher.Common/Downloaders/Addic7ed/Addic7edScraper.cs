﻿using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class Addic7edScraper : IAddic7edScraper
    {
        const string BaseUrl = "http://www.addic7ed.com";
        const string UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86";
        const string Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
       
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

        public IEnumerable<Addic7edSubtitle> SearchSubtitles(int seriesId, int season)
        {
            var url = $"{BaseUrl}/ajax_loadShow.php?show={seriesId}&season={season}&langs=|1|8|11|7|9|10|26|&hd=0&hi=0";
            var web = CreateHtmlWeb();
            var document = web.Load(url);
            var subtitleContainers = document.DocumentNode.SelectNodes("//tr[contains(@class, 'completed')]");
            var subtitles = subtitleContainers.Select(sc => new Addic7edSubtitle
            {
                Season = int.Parse(sc.SelectSingleNode("./td[1]").InnerText.Trim()),
                Episode = int.Parse(sc.SelectSingleNode("./td[2]").InnerText.Trim()),
                Version = sc.SelectSingleNode("./td[5]").InnerText.Trim(),
                DowloadLink = BaseUrl + "/" + sc.SelectSingleNode(".//a[text()='Download']").Attributes["href"].Value.Trim(),
                Language = sc.SelectSingleNode("./td[4]").InnerText.Trim(),
                HearingImpaired = sc.SelectSingleNode("./td[5]").InnerText.Trim() == "✔",
                Corrected = sc.SelectSingleNode("./td[6]").InnerText.Trim() == "✔",
                HighDefinition = sc.SelectSingleNode("./td[7]").InnerText.Trim() == "✔"
            });

            return subtitles;
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
}
