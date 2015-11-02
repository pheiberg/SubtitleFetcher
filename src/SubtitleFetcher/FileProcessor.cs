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
                logger.Error("File format", $"Can't parse episode info from {fileName}. Not on a known format.");
                return true;
            }

            if (ignoredShows.Any(s => string.Equals(s.RemoveNonAlphaNumericChars(), episodeIdentity.SeriesName.RemoveNonAlphaNumericChars(), StringComparison.OrdinalIgnoreCase)))
            {
                logger.Verbose("FileProcessor", $"Ignoring {fileName}");
                return true;
            }

            logger.Important("FileProcessor", $"Processing file {fileName}...");

            var dowloadedLanguages = fileSystem.GetDowloadedSubtitleLanguages(fileName, languageSettings.Languages);
            var languagesToDownload = languageSettings.Languages.Except(dowloadedLanguages).ToArray();

            if (!languagesToDownload.Any())
            {
                logger.Verbose("FileProcessor", $"All languages already downloaded. Skipping {fileName}.");
                return true;
            }

            var languageList = string.Join(", ", languagesToDownload);
            logger.Debug("FileProcessor", $"Looking for subtitles in: {languageList}");

            var isSuccessful = subtitleService.DownloadSubtitle(fileName , episodeIdentity, languagesToDownload);
            return isSuccessful;
        }
    }
}