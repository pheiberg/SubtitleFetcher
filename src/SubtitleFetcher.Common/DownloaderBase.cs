using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using SubtitleDownloader.Core;

namespace SubtitleFetcher.Common
{
    public abstract class DownloaderBase
    {
        private readonly string name;
        private readonly GenericDownloader downloader;

        protected DownloaderBase(string name, ILogger logger, IEpisodeParser parser, string episodeListUrlFormat, string subtitleRegex, string downloadUrlFormat) 
        {
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

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            return downloader.SearchSubtitles(query, GetShowId, SearchTimeout);
        }

        protected abstract string GetShowId(string name);

        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            throw new NotImplementedException();
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            return downloader.SaveSubtitle(subtitle, SearchTimeout);
        }

        protected WebClient GetWebClient()
        {
            return downloader.CreateWebClient(SearchTimeout);
        }
    }
}