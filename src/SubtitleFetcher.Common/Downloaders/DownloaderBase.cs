using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher.Common.Downloaders
{
    public abstract class DownloaderBase : ISubtitleDownloader
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

        public string GetName()
        {
            return _name;
        }
        
        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            if (SupportedLanguages.Any() && !query.Languages.Intersect(SupportedLanguages).Any())
            {
                _logger.Debug(_name, "The downloader only provides texts for the languages: {0}. Aborting search.", string.Join(", ", SupportedLanguages.Select(l => l.TwoLetterIsoName).ToArray()));
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
        
        public abstract IEnumerable<Language> SupportedLanguages { get; } 

        public virtual IEnumerable<IEnhancementRequest> EnhancementRequests => Enumerable.Empty<IEnhancementRequest>();
    }
}