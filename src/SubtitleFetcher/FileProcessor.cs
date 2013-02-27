using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TvShowIdentification;

namespace SubtitleFetcher
{
    public class FileProcessor
    {
        private readonly IEpisodeParser episodeParser;
        private readonly ILogger logger;
        private readonly IEnumerable<string> ignoredShows;
        private readonly ISubtitleDownloadService subtitleService;

        public FileProcessor(IEpisodeParser episodeParser, ILogger logger, IEnumerable<string> ignoredShows, ISubtitleDownloadService subtitleService)
        {
            this.episodeParser = episodeParser;
            this.logger = logger;
            this.ignoredShows = ignoredShows;
            this.subtitleService = subtitleService;
        }

        public bool ProcessFile(string fileName)
        {
            var episodeIdentity = episodeParser.ParseEpisodeInfo(Path.GetFileNameWithoutExtension(fileName));
            if (string.IsNullOrEmpty(episodeIdentity.SeriesName))
            {
                logger.Error("Can't parse episode info from {0}. Not on a known format.", fileName);
                return true;
            }

            if (ignoredShows.Any(s => string.Equals(s.RemoveNonAlphaNumericChars(), episodeIdentity.SeriesName.RemoveNonAlphaNumericChars(), StringComparison.InvariantCultureIgnoreCase)))
            {
                logger.Verbose("Ignoring {0}", fileName);
                return true;
            }

            logger.Important("Processing file {0}...", fileName);

            var isSuccessful = subtitleService.DownloadSubtitle(FileSystem.CreateSubtitleFileName(fileName) , episodeIdentity);
            return isSuccessful;
        }
    }
}