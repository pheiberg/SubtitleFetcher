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
    public class SwesubDownloader : ISubtitleDownloader
    {
        private readonly TvdbSearcher tvDbSearcher = new TvdbSearcher();
        private readonly IDictionary<string, IEnumerable<Subtitle>> cache = new Dictionary<string, IEnumerable<Subtitle>>();
        private static readonly Regex SubtitleRegex = new Regex("<a href=\"/download/(?<id>\\d{1,})/\"( rel=\"nofollow\")?.*?>(?<release>.*?)\\s*(\\(\\d*\\s?cd\\))?</a>");

        private const string DownloadUrlFormat = "http://swesub.nu/download/{0}/";
        private const string EpisodeListFormat = "http://swesub.nu/title/tt{0}";

        public string GetName()
        {
            return "Swesub";
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            throw new System.NotImplementedException();
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            var series = tvDbSearcher.FindSeriesExact(query.SerieTitle);
            if(string.IsNullOrEmpty(series.ImdbId))
                return new List<Subtitle>();

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
            var client = new WebClient();
            byte[] data = client.DownloadData(url);
            using (Stream s = new MemoryStream(data))
            using (var reader = ReaderFactory.Open(s))
            {
                while (reader.MoveToNextEntry())
                {
                    var extension = Path.GetExtension(reader.Entry.FilePath);
                    if (string.Equals(extension, ".srt", StringComparison.InvariantCultureIgnoreCase))
                    {
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
            var client = new WebClient();

            string seriesList;
            try
            {
                seriesList = client.DownloadString(url);
            }
            catch (WebException)
            {
                return Enumerable.Empty<Subtitle>();
            }
            var matches = SubtitleRegex.Matches(seriesList);
            var subtitles = (from Match match in matches
                            select new Subtitle(match.Groups["id"].Value, "Swesub", match.Groups["release"].Value, "swe")).ToList();
            cache[id] = subtitles; 
            return subtitles;
        }
    }
}