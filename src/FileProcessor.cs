using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SubtitleFetcher
{
    public class FileProcessor
    {
        private readonly EpisodeParser episodeParser;
        private readonly Logger logger;
        private readonly IEnumerable<string> ignoredShows;
        private readonly SubtitleDownloadService subtitleService;

        public FileProcessor(EpisodeParser episodeParser, Logger logger, IEnumerable<string> ignoredShows, SubtitleDownloadService subtitleService)
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
                logger.Log("Can't parse episode info from {0}. Not on a known format.");
                return true;
            }

            if (ignoredShows.Any(s => string.Equals(s.RemoveNonAlphaNumericChars(), episodeIdentity.SeriesName.RemoveNonAlphaNumericChars(), StringComparison.InvariantCultureIgnoreCase)))
            {
                logger.Log("Ignoring {0}", fileName);
                return true;
            }

            logger.Log("Processing file {0}...", fileName);

            var isSuccessful = subtitleService.DownloadSubtitle(FileSystem.CreateSubtitleFileName(fileName) , episodeIdentity);
            return isSuccessful;
        }
    }
}