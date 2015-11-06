using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders.SubDb;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IEpisodeParser episodeParser;
        private readonly ILogger logger;
        private readonly ISubtitleDownloadService subtitleService;
        private readonly IFileSystem fileSystem;
        private readonly LanguageSettings languageSettings;
        private readonly ISubDbHasher _hasher;

        public FileProcessor(IEpisodeParser episodeParser, ILogger logger, ISubtitleDownloadService subtitleService, IFileSystem fileSystem, LanguageSettings languageSettings, ISubDbHasher hasher)
        {
            this.episodeParser = episodeParser;
            this.logger = logger;
            this.subtitleService = subtitleService;
            this.fileSystem = fileSystem;
            this.languageSettings = languageSettings;
            _hasher = hasher;
        }

        public bool ProcessFile(string fileName, IEnumerable<string> ignoredShows)
        {
            var episodeIdentity = ParseReleaseIdentity(fileName);
            if (string.IsNullOrEmpty(episodeIdentity.SeriesName))
            {
                logger.Error("File format", $"Can't parse episode info from {fileName}. Not on a known format.");
                return true;
            }

            if (CheckIfShowIsIgnored(ignoredShows, episodeIdentity))
            {
                logger.Verbose("FileProcessor", $"Ignoring {fileName}");
                return true;
            }

            var languagesToDownload = DetermineLanguagesToDownload(fileName);
            if (!languagesToDownload.Any())
            {
                logger.Verbose("FileProcessor", $"All languages already downloaded. Skipping {fileName}.");
                return true;
            }
            
            var languageList = string.Join(", ", languagesToDownload);
            logger.Important("FileProcessor", $"Processing file {fileName}...");
            logger.Debug("FileProcessor", $"Looking for subtitles in: {languageList}");

            return DownloadSubtitle(fileName, episodeIdentity, languagesToDownload);
        }

        private string[] DetermineLanguagesToDownload(string fileName)
        {
            var dowloadedLanguages = GetAlreadyDownloadedLanguages(fileName);
            return languageSettings.Languages.Except(dowloadedLanguages).ToArray();
        }

        private TvReleaseIdentity ParseReleaseIdentity(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var tvReleaseIdentity = episodeParser.ParseEpisodeInfo(fileNameWithoutExtension);
            tvReleaseIdentity.FileHash = _hasher.ComputeHash(fileName);
            return tvReleaseIdentity;
        }

        private static bool CheckIfShowIsIgnored(IEnumerable<string> ignoredShows, TvReleaseIdentity tvReleaseIdentity)
        {
            return ignoredShows.Any(s => string.Equals(s.RemoveNonAlphaNumericChars(), tvReleaseIdentity.SeriesName.RemoveNonAlphaNumericChars(), StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<string> GetAlreadyDownloadedLanguages(string fileName)
        {
            return fileSystem.GetDowloadedSubtitleLanguages(fileName, languageSettings.Languages);
        }

        private bool DownloadSubtitle(string fileName, TvReleaseIdentity tvReleaseIdentity, string[] languagesToDownload)
        {
            return subtitleService.DownloadSubtitle(fileName , tvReleaseIdentity, languagesToDownload);
        }
    }
}