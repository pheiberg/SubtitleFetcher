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
        private readonly IEpisodeParser _episodeParser;
        private readonly ILogger _logger;
        private readonly ISubtitleDownloadService _subtitleService;
        private readonly IFileSystem _fileSystem;
        private readonly LanguageSettings _languageSettings;
        private readonly ISubDbHasher _hasher;

        public FileProcessor(IEpisodeParser episodeParser, ILogger logger, ISubtitleDownloadService subtitleService, 
                             IFileSystem fileSystem, LanguageSettings languageSettings, ISubDbHasher hasher)
        {
            _episodeParser = episodeParser;
            _logger = logger;
            _subtitleService = subtitleService;
            _fileSystem = fileSystem;
            _languageSettings = languageSettings;
            _hasher = hasher;
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
            
            var languageList = string.Join(", ", languagesToDownload);
            _logger.Important("FileProcessor", $"Processing file {fileName}...");
            _logger.Debug("FileProcessor", $"Looking for subtitles in: {languageList}");

            return DownloadSubtitle(fileName, episodeIdentity, languagesToDownload);
        }

        private string[] DetermineLanguagesToDownload(string fileName)
        {
            var dowloadedLanguages = GetAlreadyDownloadedLanguages(fileName);
            return _languageSettings.Languages.Except(dowloadedLanguages).ToArray();
        }

        private TvReleaseIdentity ParseReleaseIdentity(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var tvReleaseIdentity = _episodeParser.ParseEpisodeInfo(fileNameWithoutExtension);
            tvReleaseIdentity.FileHash = _hasher.ComputeHash(fileName);
            return tvReleaseIdentity;
        }

        private static bool CheckIfShowIsIgnored(IEnumerable<string> ignoredShows, 
            TvReleaseIdentity tvReleaseIdentity)
        {
            return ignoredShows.Any(s => string.Equals(s.RemoveNonAlphaNumericChars(), 
                tvReleaseIdentity.SeriesName.RemoveNonAlphaNumericChars(), 
                StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<string> GetAlreadyDownloadedLanguages(string fileName)
        {
            return _fileSystem.GetDowloadedSubtitleLanguages(fileName, _languageSettings.Languages);
        }

        private bool DownloadSubtitle(string fileName, TvReleaseIdentity tvReleaseIdentity, 
            IEnumerable<string> languagesToDownload)
        {
            return _subtitleService.DownloadSubtitle(fileName , tvReleaseIdentity, languagesToDownload);
        }
    }
}