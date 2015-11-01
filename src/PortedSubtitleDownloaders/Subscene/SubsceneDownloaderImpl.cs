using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using PortedSubtitleDownloaders.Legacy;

namespace PortedSubtitleDownloaders.Subscene
{
    public class SubsceneDownloaderImpl
    {
        private string[] _languageCodes;
        private int _searchTimeout;
        private static readonly string QueryUrl = "http://v2.subscene.com/s.aspx?q=";
        private static readonly string DownloadListUrlBase = "http://v2.subscene.com";
        private static readonly string DownloadUrlBase = "http://subscene.com";

        public int SearchTimeout
        {
            get
            {
                return _searchTimeout;
            }
            set
            {
                _searchTimeout = value;
            }
        }

        public static Dictionary<string, int> LanguageIds => new Dictionary<string, int>()
        {
            { "Albanian", 1 },
            { "Arabic", 2 },
            { "Bengali", 54 },
            { "Bulgarian", 5 },
            { "Catalan", 49 },
            { "Chinese", 7 },
            { "Croatian", 8 },
            { "Czech", 9 },
            { "Danish", 10 },
            { "Dutch", 11 },
            { "English", 13 },
            { "Estonian", 16 },
            { "Farsi", 46 },
            { "Persian", 46 },
            { "Finnish", 17 },
            { "French", 18 },
            { "German", 19 },
            { "Greek", 21 },
            { "Hebrew", 22 },
            { "Hindi", 51 },
            { "Hungarian", 23 },
            { "Icelandic", 25 },
            { "Indonesian", 44 },
            { "Italian", 26 },
            { "Japanese", 27 },
            { "Korean", 28 },
            { "Kurdish", 52 },
            { "Latvian", 29 },
            { "Lithuanian", 43 },
            { "Macedonian", 48 },
            { "Malay", 50 },
            { "Norwegian", 30 },
            { "Polish", 31 },
            { "Portuguese", 32 },
            { "Romanian", 33 },
            { "Russian", 34 },
            { "Serbian", 35 },
            { "Slovak", 36 },
            { "Slovenian", 37 },
            { "Spanish", 38 },
            { "Swedish", 39 },
            { "Tagalog", 53 },
            { "Thai", 40 },
            { "Turkish", 41 },
            { "Ukranian", 56 },
            { "Ukrainian", 56 },
            { "Urdu", 42 },
            { "Vietnamese", 45 }
        };

        public string GetName()
        {
            return "Subscene";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            _languageCodes = query.LanguageCodes;
            List<Subtitle> list = new List<Subtitle>();
            HtmlNodeCollection htmlNodeCollection = new HtmlWeb
            {
                UseCookies = true,
                PreRequest = OnPreRequest
            }.Load(QueryUrl + query.Query).DocumentNode.SelectNodes("//a");
            if (htmlNodeCollection == null)
                return new List<Subtitle>(0);
            foreach (HtmlNode subtitleLink in htmlNodeCollection)
            {
                string attributeValue = subtitleLink.GetAttributeValue("href", string.Empty);
                if (!attributeValue.Contains("subtitle-"))
                    continue;

                string languageCode;
                string subtitleName;
                if (ParseLanguageAndSubtitleName(subtitleLink, out languageCode, out subtitleName) && query.HasLanguageCode(languageCode))
                {
                    Subtitle subtitle = new Subtitle(attributeValue, subtitleName, subtitleName, languageCode);
                    list.Add(subtitle);
                }
            }
            return list;
        }

        public IEnumerable<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            string se = $"s{query.Season:00}";
            string ep = $"e{query.Episode:00}";
            string serieTitle = query.SerieTitle;
            char[] chArray = {' '};
            var seriesNameParts = serieTitle.Split(chArray);
            string episodeName = seriesNameParts.Aggregate("", (current, next) => current + next + ".");
            SearchQuery query1 = new SearchQuery(episodeName + se + ep) {LanguageCodes = query.LanguageCodes};
            var list1 = SearchSubtitles(query1).ToList();
            SearchQuery query2 = new SearchQuery(string.Concat(query.SerieTitle, " ", query.Season,
                $"{query.Episode:00}")) {LanguageCodes = query.LanguageCodes};
            var list2 = SearchSubtitles(query2);
            list1.AddRange(list2);
            return list1;
        }
        
        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string archiveFile = DownloadSubtitle(subtitle.Id);
            if (archiveFile != null)
                return FileUtils.ExtractFilesFromZipOrRarFile(archiveFile);
            throw new Exception("Subtitle download failed! Downloading returned unexpected result!");
        }

        private static string DownloadSubtitle(string subtitleUrl)
        {
            try
            {
                HtmlNodeCollection htmlNodeCollection = new HtmlWeb().Load(DownloadListUrlBase + subtitleUrl).DocumentNode.SelectNodes("//a");
                if (htmlNodeCollection.Count == 0)
                    throw new Exception("Subtitle download failed! No download link available");

                foreach (HtmlNode htmlNode in htmlNodeCollection)
                {
                    string attributeValue = htmlNode.GetAttributeValue("href", string.Empty);
                    if (!attributeValue.StartsWith("/subtitle/download"))
                        continue;

                    string address = DownloadUrlBase + attributeValue;
                    string tempFileName = FileUtils.GetTempFileName();
                    new WebClient().DownloadFile(address, tempFileName);
                    return tempFileName;
                }
            }
            catch
            {
            }
            return null;
        }

        private static bool ParseLanguageAndSubtitleName(HtmlNode subtitleLink, out string languageCode, out string subtitleName)
        {
            languageCode = "";
            subtitleName = "";
            HtmlNodeCollection childNodes = subtitleLink.ChildNodes;
            bool flag = false;
            foreach (HtmlNode htmlNode in childNodes)
            {
                if (!htmlNode.Name.Equals("span"))
                    continue;

                string languageName = htmlNode.InnerText.Trim();
                if (Languages.IsSupportedLanguageName(languageName))
                {
                    languageCode = Languages.GetLanguageCode(languageName);
                    flag = true;
                }
                else if (flag)
                {
                    string[] strArray = languageName.Split('(');
                    subtitleName = strArray[0].TrimEnd(' ');
                }
            }
            return flag;
        }

        private bool OnPreRequest(HttpWebRequest request)
        {
            SetRequestTimeout(request);
            SetLanguagesCookie(request);
            return true;
        }

        private void SetRequestTimeout(WebRequest request)
        {
            if (_searchTimeout > 0)
            {
                request.Timeout = _searchTimeout*1000;
            }
        }

        private void SetLanguagesCookie(HttpWebRequest request)
        {
            var languageIds = GetLanguageIds();
            Cookie cookie = new Cookie("subscene_sLanguageIds", languageIds, "/", "subscene.com");
            request.CookieContainer.Add(cookie);
        }

        private string GetLanguageIds()
        {
            return string.Join("-", _languageCodes.Select(Languages.GetLanguageName));
        }
    }
}
