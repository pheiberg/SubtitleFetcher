using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SubtitleFetcher
{
    public class FileProcessor
    {
        private readonly EpisodeParser episodeParser;
        private static readonly string[] AcceptedExtensions = new[] { ".avi", ".mkv", ".mp4" };
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
            if (!IsFileOfAcceptableType(fileName)) 
                return true;

            if (IsFileAlreadyDownloaded(fileName)) 
                return true;

            var episodeIdentity = episodeParser.ParseEpisodeInfo(Path.GetFileNameWithoutExtension(fileName));
            if (string.IsNullOrEmpty(episodeIdentity.SeriesName))
            {
                logger.Log("Can't parse episode info from {0}. Not on a known format.");
                return true;
            }

            if (ignoredShows.Any(s => string.Equals(s, episodeIdentity.SeriesName)))
            {
                logger.Log("Ignoring {0}", fileName);
                return true;
            }

            logger.Log("Processing file {0}...", fileName);

            var targetLocation = GetTargetFileNamePrefix(fileName);
            var isSuccessful = subtitleService.DownloadSubtitle(targetLocation + ".srt", episodeIdentity);
            return isSuccessful;
        }

        private static bool IsFileAlreadyDownloaded(string fileName)
        {
            var targetLocation = GetTargetFileNamePrefix(fileName);
            return File.Exists(targetLocation + ".srt") || File.Exists(targetLocation + ".nosrt");
        }

        private static string GetTargetFileNamePrefix(string fileName)
        {
            var path = Path.GetDirectoryName(fileName);
            var file = Path.GetFileNameWithoutExtension(fileName);
            var targetLocation = Path.Combine(path, file);
            return targetLocation;
        }

        private static bool IsFileOfAcceptableType(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            return AcceptedExtensions.Contains(ext);
        }
    }
}