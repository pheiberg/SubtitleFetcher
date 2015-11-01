using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SharpCompress.Reader;

namespace SubtitleFetcher.Common
{
    public class GenericDownloader
    {
        private readonly string name;
        private readonly ILogger logger;
        private readonly IEpisodeParser parser;
        private readonly string episodeListUrlFormat;
        private readonly Regex subtitleRegex;
        private readonly IDictionary<string, IEnumerable<Subtitle>> cachedSubtitleLists = new Dictionary<string, IEnumerable<Subtitle>>();
        private readonly string downloadUrlFormat;

        public GenericDownloader(string name, ILogger logger, IEpisodeParser parser, string episodeListUrlFormat, string subtitleRegex, string downloadUrlFormat)
        {
            this.name = name;
            this.logger = logger;
            this.episodeListUrlFormat = episodeListUrlFormat;
            this.downloadUrlFormat = downloadUrlFormat;
            this.parser = parser;
            this.subtitleRegex = new Regex(subtitleRegex);
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query, Func<string, string> getShowId, int timeout)
        {
            var id = getShowId(query.SerieTitle);
            if (string.IsNullOrEmpty(id))
            {
                logger.Debug(name, "Could not find show id");
                return new List<Subtitle>();
            }

            var subtitles = ListSeriesSubtitles(id, timeout);
            var matches = from title in subtitles
                          let identity = parser.ParseEpisodeInfo(title.FileName)
                          where identity.Season == query.Season && identity.Episode == query.Episode
                          select title;
            return matches.ToList();
        }

        public IEnumerable<Subtitle> ListSeriesSubtitles(string id, int timeout)
        {
            IEnumerable<Subtitle> cached;
            if (cachedSubtitleLists.TryGetValue(id, out cached))
                return cached;

            var url = string.Format(episodeListUrlFormat, id);
            logger.Debug(name, "Requesting TV show page from {0}", url);

            string seriesList;
            try
            {
                seriesList = CreateWebClient(timeout).DownloadString(url);
            }
            catch (WebException ex)
            {
                LogWebClientError(ex);
                return Enumerable.Empty<Subtitle>();
            }
            var matches = subtitleRegex.Matches(seriesList);
            var subtitles = (from Match match in matches
                             select new Subtitle(match.Groups["id"].Value, name, match.Groups["release"].Value, "swe")).ToList();
            cachedSubtitleLists[id] = subtitles;
            return subtitles;
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle, int timeout)
        {
            var url = string.Format(downloadUrlFormat, subtitle.Id);
            logger.Debug(name, "Saving file from {0}", url);
            var client = CreateWebClient(timeout);
            byte[] data = client.DownloadData(url);
            using (Stream s = new MemoryStream(data))
            using (var reader = ReaderFactory.Open(s))
            {
                logger.Debug(name, "Unpacking {0} file", reader.ArchiveType);
                while (reader.MoveToNextEntry())
                {
                    var extension = Path.GetExtension(reader.Entry.Key);
                    if (!string.Equals(extension, ".srt", StringComparison.OrdinalIgnoreCase)) 
                        continue;

                    logger.Debug(name, "Extracting file {0}", reader.Entry.Key);
                    var filePath = Path.Combine(Path.GetTempPath(), subtitle.FileName + ".srt");
                    reader.WriteEntryTo(filePath);
                    return new List<FileInfo> { new FileInfo(filePath) };
                }
            }
            return new List<FileInfo>();
        }

        public WebClient CreateWebClient(int timeout)
        {
            var webClient = new WebDownloader();
            if (timeout > 0)
            {
                webClient.Timeout = timeout;
            }
            return webClient;
        }

        public void LogWebClientError(WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null &&
                ((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.NotFound)
            {
                logger.Debug(name, "Show page could not be found");
            }
            else
            {
                logger.Important(name, "Error occured in downloader {0}: {1}", name, ex.Message);
            }
        }
    }
}