using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace SubtitleFetcher.Common
{
    public abstract class DownloaderBase : IExtendedSubtitleDownloader
    {
        private readonly string name;
        private readonly GenericDownloader downloader;
        private readonly ILogger logger;

        protected DownloaderBase(string name, ILogger logger, IEpisodeParser parser, string episodeListUrlFormat, string subtitleRegex, string downloadUrlFormat)
        {
            this.logger = logger;
            downloader = new GenericDownloader(name, logger, parser, episodeListUrlFormat, subtitleRegex, downloadUrlFormat);
            this.name = name;
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
            return name;
        }
        
        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            if (LanguageLimitations.Any() && !query.LanguageCodes.Intersect(LanguageLimitations).Any())
            {
                logger.Debug(name, "The downloader only provides texts for the languages: {0}. Aborting search.", string.Join(", ", LanguageLimitations.ToArray()));
                return new List<Subtitle>();
            }
            return downloader.SearchSubtitles(query, GetShowId, SearchTimeout);
        }

        protected abstract string GetShowId(string name);

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            return downloader.SaveSubtitle(subtitle, SearchTimeout);
        }

        protected WebClient GetWebClient()
        {
            return downloader.CreateWebClient(SearchTimeout);
        }

        IEnumerable<FileInfo> ISubtitleDownloader.SaveSubtitle(Subtitle subtitle)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<string> LanguageLimitations
        {
            get { return Enumerable.Empty<string>(); }
        }
    }
}