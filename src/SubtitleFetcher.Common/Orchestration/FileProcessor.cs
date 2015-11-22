using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher.Common.Orchestration
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IEpisodeParser _episodeParser;
        private readonly ILogger _logger;
        private readonly ISubtitleDownloadService _subtitleService;
        private readonly IFileOperations _fileOperations;
        private readonly LanguageSettings _languageSettings;

        public FileProcessor(IEpisodeParser episodeParser, ILogger logger, ISubtitleDownloadService subtitleService, 
                             IFileOperations fileOperations, LanguageSettings languageSettings)
        {
            _episodeParser = episodeParser;
            _logger = logger;
            _subtitleService = subtitleService;
            _fileOperations = fileOperations;
            _languageSettings = languageSettings;
        }

        public bool ProcessFile(string fileName, IEnumerable<string> ignoredShows)
        {
            var episodeIdentity = ParseReleaseIdentity(fileName);
            if (string.IsNullOrEmpty(episodeIdentity.SeriesName))
            {
                _logger.Error("File format", $"Can't parse episode info from {fileName}. Not on a known format.");
                return true;
            }

            if (CheckIfShowIsIgnored(ignoredShows, episodeIdentity))
            {
                _logger.Verbose("FileProcessor", $"Ignoring {fileName}");
                return true;
            }

            var languagesToDownload = DetermineLanguagesToDownload(fileName);
            if (!languagesToDownload.Any())
            {
                _logger.Verbose("FileProcessor", $"All languages already downloaded. Skipping {fileName}.");
                return true;
            }
            
            var languageList = string.Join(", ", languagesToDownload.Select(l => l.Name));
            _logger.Important("FileProcessor", $"Processing file {fileName}...");
            _logger.Debug("FileProcessor", $"Looking for subtitles in: {languageList}");

            return DownloadSubtitle(fileName, episodeIdentity, languagesToDownload);
        }

        private Language[] DetermineLanguagesToDownload(string fileName)
        {
            var dowloadedLanguages = GetAlreadyDownloadedLanguages(fileName);
            return _languageSettings.Languages.Except(dowloadedLanguages).ToArray();
        }

        private TvReleaseIdentity ParseReleaseIdentity(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            return _episodeParser.ParseEpisodeInfo(fileNameWithoutExtension);
        }

        private static bool CheckIfShowIsIgnored(IEnumerable<string> ignoredShows, 
            TvReleaseIdentity tvReleaseIdentity)
        {
            return ignoredShows.Any(s => string.Equals(s.RemoveNonAlphaNumericChars(), 
                tvReleaseIdentity.SeriesName.RemoveNonAlphaNumericChars(), 
                StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<Language> GetAlreadyDownloadedLanguages(string fileName)
        {
            return _fileOperations.GetDowloadedSubtitleLanguages(fileName, _languageSettings.Languages);
        }

        private bool DownloadSubtitle(string fileName, TvReleaseIdentity tvReleaseIdentity, 
            IEnumerable<Language> languagesToDownload)
        {
            return _subtitleService.DownloadSubtitle(fileName , tvReleaseIdentity, languagesToDownload);
        }
    }
}