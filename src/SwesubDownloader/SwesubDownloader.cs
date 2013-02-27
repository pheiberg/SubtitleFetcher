using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SharpCompress.Reader;
using SubtitleDownloader.Core;
using TvShowIdentification;

namespace SwesubDownloader
{
    public class SwesubDownloader : ISubtitleDownloader, IDownloadCapabilitiesProvider
    {
        private readonly ILogger logger;
        private readonly ITvdbSearcher tvdbSearcher;
        private readonly IDictionary<string, IEnumerable<Subtitle>> cache = new Dictionary<string, IEnumerable<Subtitle>>();
        private static readonly Regex SubtitleRegex = new Regex("<a href=\"/download/(?<id>\\d{1,})/\"( rel=\"nofollow\")?.*?>(?<release>.*?)\\s*(\\(\\d*\\s?cd\\))?</a>");

        private const string DownloadUrlFormat = "http://swesub.nu/download/{0}/";
        private const string EpisodeListFormat = "http://swesub.nu/title/tt{0}";

        public SwesubDownloader(ILogger logger, ITvdbSearcher tvdbSearcher)
        {
            this.logger = logger;
            this.tvdbSearcher = tvdbSearcher;
        }

        public string GetName()
        {
            return "Swesub";
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            if (!query.LanguageCodes.Contains("swe"))
            {
                logger.Debug("The Swesub downloader only provides swedish texts. Aborting search.");
                return new List<Subtitle>();
            }

            logger.Debug("Looking up the show imdb id");
            var series = tvdbSearcher.FindSeriesExact(query.SerieTitle);
            if (string.IsNullOrEmpty(series.ImdbId))
            {
                logger.Debug("Could not find imdb id");
                return new List<Subtitle>();
            }

            logger.Debug(string.Format("Imdb id found: {0}", series.ImdbId));
            var parser = new EpisodeParser();
            var subtitles = ListSeriesSubtitles(series.ImdbId);
            var matches = from title in subtitles
                          let identity = parser.ParseEpisodeInfo(title.FileName)
                          where identity.Season == query.Season && identity.Episode == query.Episode
                          select title;
            return matches.ToList();
        }

        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            return ListSeriesSubtitles(query.ImdbId).ToList();
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            var url = string.Format(DownloadUrlFormat, subtitle.Id);
            logger.Debug("Saving file from {0}", url);
            var client = CreateWebClient();
            byte[] data = client.DownloadData(url);
            using (Stream s = new MemoryStream(data))
            using (var reader = ReaderFactory.Open(s))
            {
                logger.Debug("Unpacking {0} file", reader.ArchiveType);
                while (reader.MoveToNextEntry())
                {
                    var extension = Path.GetExtension(reader.Entry.FilePath);
                    if (string.Equals(extension, ".srt", StringComparison.InvariantCultureIgnoreCase))
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
            get;
            set;
        }

        public IEnumerable<Subtitle> ListSeriesSubtitles(string imdbId)
        {
            var id = imdbId.TrimStart('t');
            IEnumerable<Subtitle> cached;
            if (cache.TryGetValue(id, out cached))
                return cached;

            var url = string.Format(EpisodeListFormat, id);
            logger.Debug("Requesting TV show page from {0}", url);

            string seriesList;
            try
            {
                seriesList = CreateWebClient().DownloadString(url);
            }
            catch (WebException ex)
            {
                LogWebClientError(ex);
                return Enumerable.Empty<Subtitle>();
            }
            var matches = SubtitleRegex.Matches(seriesList);
            var subtitles = (from Match match in matches
                             select new Subtitle(match.Groups["id"].Value, "Swesub", match.Groups["release"].Value, "swe")).ToList();
            cache[id] = subtitles;
            return subtitles;
        }

        private WebClient CreateWebClient()
        {
            var webClient = new WebDownloader();
            if(SearchTimeout > 0)
            {
                webClient.Timeout = SearchTimeout;
            }
            return webClient;
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

        public bool CanHandleEpisodeSearchQuery
        {
            get { return true; }
        }

        public bool CanHandleImdbSearchQuery
        {
            get { return false; }
        }

        public bool CanHandleSearchQuery
        {
            get { return false; }
        }
    }
}