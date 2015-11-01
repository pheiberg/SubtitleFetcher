using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using PortedSubtitleDownloaders.Legacy;

namespace PortedSubtitleDownloaders.S4U
{
    public class S4UDownloaderImpl
    {
        private const string ApiKey = "Oiyb6TR41fga61j";
        private static readonly string SwedishLanguageCode = Languages.GetLanguageCode("Swedish");

        public int SearchTimeout { get; set; }

        public string GetName()
        {
            return "S4U.se";
        }

        public IEnumerable<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            var list = new List<Subtitle>();
            if (!query.HasLanguageCode("swe"))
                return list;
            string url = query.TvdbId.HasValue ? 
                GetSerieQueryTvdbIdSearchUrl(query.TvdbId.Value, query.Season, query.Episode) : 
                GetSerieQuerySearchUrl(query.SerieTitle, query.Season, query.Episode);
            list.AddRange(GetResultsFromUrl(url));
            return list;
        }
        
        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string id = subtitle.Id;
            string tempFileName = FileUtils.GetTempFileName();
            new WebClient().DownloadFile(id, tempFileName);
            IEnumerable<FileInfo> fromZipOrRarFile = FileUtils.ExtractFilesFromZipOrRarFile(tempFileName);
            return fromZipOrRarFile.Where(IsSupportedFileType).ToList();
        }

        private static bool IsSupportedFileType(FileInfo fileInfo)
        {
            return fileInfo.Extension.Equals(".srt") || fileInfo.Extension.Equals(".sub");
        }

        private IEnumerable<Subtitle> GetResultsFromUrl(string url)
        {
            var xmlDocument = GetXmlDocument(url);
            var subElements = xmlDocument.GetElementsByTagName("sub");
            var subtitles = from XmlNode xmlNode in subElements
                            select CreateSubtitle(xmlNode);
            return subtitles;
        }

        private static Subtitle CreateSubtitle(XmlNode xmlNode)
        {
            string id = null;
            string programName = null;
            string fileName = null;

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.LocalName.Equals("download_zip"))
                {
                    id = node.InnerText;
                }
                if (node.LocalName.Equals("ep_title"))
                {
                    programName = node.InnerText;
                }
                if (node.LocalName.Equals("file_name"))
                {
                    fileName = node.InnerText;
                }
            }
            var subtitle = new Subtitle(id, programName, fileName, SwedishLanguageCode);
            return subtitle;
        }

        private XmlDocument GetXmlDocument(string url)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            SetRequestTimeout(httpWebRequest);
            using (var responseStream = httpWebRequest.GetResponse().GetResponseStream())
            {
                var xmlDocument = GetXmlDocument(responseStream);
                return xmlDocument;
            }
        }

        private void SetRequestTimeout(WebRequest httpWebRequest)
        {
            if (SearchTimeout > 0)
            {
                httpWebRequest.Timeout = SearchTimeout * 1000;
            }
        }

        private static XmlDocument GetXmlDocument(Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(reader);
                return xmlDocument;
            }
        }

        private string GetSerieQueryTvdbIdSearchUrl(int tvdbid, int season, int episode)
        {
            return $"http://api.s4u.se/1.0/{ApiKey}/xml/serie/tvdb/{tvdbid}/season={season}&episode={episode}";
        }

        private string GetSerieQuerySearchUrl(string query, int season, int episode)
        {
            return $"http://api.s4u.se/1.0/{ApiKey}/xml/serie/title/{query}/season={season}&episode={episode}";
        }
    }
}
