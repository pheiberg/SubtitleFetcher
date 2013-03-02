using System.Collections.Generic;
using SubtitleFetcher.Common;

namespace SwesubDownloader
{
    public class SwesubDownloader : DownloaderBase
    {
        private readonly ILogger logger;
        private readonly ITvdbSearcher tvdbSearcher;
        private const string SubtitleRegexExpression = "<a href=\"/download/(?<id>\\d{1,})/\"( rel=\"nofollow\")?.*?>(?<release>.*?)\\s*(\\(\\d*\\s?cd\\))?</a>";
        private const string DownloadUrlFormat = "http://swesub.nu/download/{0}/";
        private const string EpisodeListFormat = "http://swesub.nu/title/{0}";
        private const string Name = "Swesub";

        public SwesubDownloader(ILogger logger, ITvdbSearcher tvdbSearcher, IEpisodeParser parser)
            : base(Name, logger, parser, EpisodeListFormat, SubtitleRegexExpression, DownloadUrlFormat)
        {
            this.logger = logger;
            this.tvdbSearcher = tvdbSearcher;
        }

        protected override string GetShowId(string name)
        {
            logger.Debug(Name, "Looking up the show imdb id");
            var series = tvdbSearcher.FindSeriesExact(name);
            if(string.IsNullOrEmpty(series.ImdbId))
            {
                logger.Debug(Name, "No Imdb id found");
                return null;
            }

            logger.Debug(Name, "Imdb id found: {0}", series.ImdbId);
            return series.ImdbId;
        }

        public override bool CanHandleEpisodeSearchQuery
        {
            get { return true; }
        }

        public override bool CanHandleImdbSearchQuery
        {
            get { return false; }
        }

        public override bool CanHandleSearchQuery
        {
            get { return false; }
        }

        public override IEnumerable<string> LanguageLimitations
        {
            get { return new []{ "swe" }; }
        }
    }
}