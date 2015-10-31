using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SubtitleFetcher.Common
{
    public abstract class DownloaderBase : IExtendedSubtitleDownloader
    {
        private readonly string _name;
        private readonly GenericDownloader _downloader;
        private readonly ILogger _logger;

        protected DownloaderBase(string name, ILogger logger, IEpisodeParser parser, string episodeListUrlFormat, string subtitleRegex, string downloadUrlFormat)
        {
            _logger = logger;
            _downloader = new GenericDownloader(name, logger, parser, episodeListUrlFormat, subtitleRegex, downloadUrlFormat);
            _name = name;
        }

        public int SearchTimeout
        {
            get; set;
        }

        public abstract bool CanHandleEpisodeSearchQuery { get; }
        public abstract bool CanHandleImdbSearchQuery { get; }
        public abstract bool CanHandleSearchQuery { get; }

        public string GetName()
        {
            return _name;
        }
        
        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            if (LanguageLimitations.Any() && !query.LanguageCodes.Intersect(LanguageLimitations).Any())
            {
                _logger.Debug(_name, "The downloader only provides texts for the languages: {0}. Aborting search.", string.Join(", ", LanguageLimitations.ToArray()));
                return Enumerable.Empty<Subtitle>();
            }
            return _downloader.SearchSubtitles(query, GetShowId, SearchTimeout);
        }

        protected abstract string GetShowId(string name);

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            return _downloader.SaveSubtitle(subtitle, SearchTimeout);
        }

        protected WebClient GetWebClient()
        {
            return _downloader.CreateWebClient(SearchTimeout);
        }

        public virtual IEnumerable<string> LanguageLimitations => Enumerable.Empty<string>();
    }
}