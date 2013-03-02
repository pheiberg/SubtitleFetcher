using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SharpCompress.Reader;
using SubtitleDownloader.Core;
using SubtitleFetcher.Common;

namespace UndertexterSeDownloader
{
    public class UndertexterSeDownloader : ISubtitleDownloader
    {
        private const string GetShowsPage = "http://undertexter.se";
        private const string ShowPage = "http://www.undertexter.se/?p=serier&id={0}";
        private const string DownloadUrlFormat = "http://www.undertexter.se/laddatext.php?id={0}";
        private readonly IDictionary<int, IEnumerable<Subtitle>> cache = new Dictionary<int, IEnumerable<Subtitle>>();
        private readonly IEpisodeParser parser;
        private readonly ILogger logger;
        private static readonly Regex SubtitleExpression = new Regex("<a.*title=\"(?<release>.*)\"\\salt=\".*\"\\shref=\"laddatext.php\\?id=(?<id>\\d+)\">");
        private static readonly Regex ShowListExpression = new Regex("<option value=\\\"http://www.undertexter.se/\\?p=serier&id=(?<id>\\d+)\\\"\\s?>(?<name>[\\w\\W]*?)</option>");

        public UndertexterSeDownloader(ILogger logger, IEpisodeParser parser)
        {
            this.logger = logger;
            this.parser = parser;
        }

        public string GetName()
        {
            return "undertexter.se";
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            var shows = GetAllSeries();
            var matching = shows.SingleOrDefault(s => string.Equals(s.Name, query.SerieTitle, StringComparison.OrdinalIgnoreCase));
            if(matching == null)
                return new List<Subtitle>();

            var subtitles = GetAllSubtitlesForShow(matching.Id);
            var matches = from title in subtitles
                          let identity = parser.ParseEpisodeInfo(title.FileName)
                          where identity.Season == query.Season && identity.Episode == query.Episode
                          select title;
            return matches.ToList();
        }

        private IEnumerable<Series> GetAllSeries()
        {
            var client = new WebClient();
            var page = client.DownloadString(GetShowsPage);
            var matches = ShowListExpression.Matches(page);
            return from Match match in matches 
                   let id = int.Parse(match.Groups["id"].Value.Trim()) 
                   let name = match.Groups["name"].Value.Trim(' ', '\n').Replace("&", "and").Replace(":", "") 
                   select new Series(id, name);
        }

        private IEnumerable<Subtitle> GetAllSubtitlesForShow(int id)
        {
            IEnumerable<Subtitle> cached;
            if (cache.TryGetValue(id, out cached))
                return cached;

            var url = string.Format(ShowPage, id);
            logger.Debug("Requesting TV show page from {0}", url);

            string seriesList;
            try
            {
                seriesList = new WebClient().DownloadString(url);
            }
            catch (WebException ex)
            {
                LogWebClientError(ex);
                return Enumerable.Empty<Subtitle>();
            }
            var matches = SubtitleExpression.Matches(seriesList);
            var subtitles = (from Match match in matches
                             select new Subtitle(match.Groups["id"].Value, GetName(), match.Groups["release"].Value, "swe")).ToList();
            cache[id] = subtitles;
            return subtitles;
        }

        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            throw new NotImplementedException();
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            var url = string.Format(DownloadUrlFormat, subtitle.Id);
            logger.Debug("Saving file from {0}", url);
            var client = new WebClient();
            byte[] data = client.DownloadData(url);
            using (Stream s = new MemoryStream(data))
            using (var reader = ReaderFactory.Open(s))
            {
                logger.Debug("Unpacking {0} file", reader.ArchiveType);
                while (reader.MoveToNextEntry())
                {
                    var extension = Path.GetExtension(reader.Entry.FilePath);
                    if (string.Equals(extension, ".srt", StringComparison.OrdinalIgnoreCase))
                    {
                        logger.Debug("Extracting file {0}", reader.Entry.FilePath);
                        var filePath = Path.Combine(Path.GetTempPath(), subtitle.FileName + ".srt");
                        reader.WriteEntryTo(filePath);
                        return new List<FileInfo> { new FileInfo(filePath) };
                    }
                }
            }
            return new List<FileInfo>();
        }

        public int SearchTimeout
        {
            get; set;
        }

        private class Series
        {
            private readonly int id;
            private readonly string name;

            public Series(int id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public string Name
            {
                get { return name; }
            }

            public int Id
            {
                get { return id; }
            }
        }

        private void LogWebClientError(WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null &&
                ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
            {
                logger.Debug("Show page could not be found");
            }
            else
            {
                logger.Important("Error occured in downloader {0}: {1}", GetName(), ex.Message);
            }
        }
    }
}
