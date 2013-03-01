using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common;

namespace SubtitleFetcher
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IEpisodeParser episodeParser;
        private readonly ILogger logger;
        private readonly ISubtitleDownloadService subtitleService;
        private readonly IFileSystem fileSystem;
        private readonly LanguageSettings languageSettings;

        public FileProcessor(IEpisodeParser episodeParser, ILogger logger, ISubtitleDownloadService subtitleService, IFileSystem fileSystem, LanguageSettings languageSettings)
        {
            this.episodeParser = episodeParser;
            this.logger = logger;
            this.subtitleService = subtitleService;
            this.fileSystem = fileSystem;
            this.languageSettings = languageSettings;
        }

        public bool ProcessFile(string fileName, IEnumerable<string> ignoredShows)
        {
            var episodeIdentity = episodeParser.ParseEpisodeInfo(Path.GetFileNameWithoutExtension(fileName));
            if (string.IsNullOrEmpty(episodeIdentity.SeriesName))
            {
                logger.Error("Can't parse episode info from {0}. Not on a known format.", fileName);
                return true;
            }

            if (ignoredShows.Any(s => string.Equals(s.RemoveNonAlphaNumericChars(), episodeIdentity.SeriesName.RemoveNonAlphaNumericChars(), StringComparison.OrdinalIgnoreCase)))
            {
                logger.Verbose("Ignoring {0}", fileName);
                return true;
            }

            logger.Important("Processing file {0}...", fileName);

            var dowloadedLanguages = fileSystem.GetDowloadedSubtitleLanguages(fileName, languageSettings.Languages);
            var languagesToDowload = languageSettings.Languages.Except(dowloadedLanguages).ToArray();
            logger.Debug("Looking for subtitles in: {0}", string.Join(", ", languagesToDowload));
            var isSuccessful = subtitleService.DownloadSubtitle(fileName , episodeIdentity, languagesToDowload);
            return isSuccessful;
        }
    }
}