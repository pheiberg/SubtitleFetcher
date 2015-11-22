using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common.Downloaders;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher.Common.Orchestration
{
    public class SubtitleDownloaderWrapper : IEpisodeSubtitleDownloader
    {
        private readonly ISubtitleDownloader _downloader;
        private readonly IEpisodeParser _nameParser;
        private readonly ILogger _logger;
        private readonly IFileOperations _fileOperations;

        public SubtitleDownloaderWrapper(ISubtitleDownloader downloader, IEpisodeParser nameParser, ILogger logger, IFileOperations fileOperations)
        {
            _downloader = downloader;
            _nameParser = nameParser;
            _logger = logger;
            _fileOperations = fileOperations;
        }

        public string Name => _downloader.GetName();

        public IEnumerable<Subtitle> SearchSubtitle(TvReleaseIdentity tvReleaseIdentity, IEnumerable<Language> languages)
        {
            var languageArray = languages.ToArray();
            var query = CreateSearchQuery(tvReleaseIdentity, languageArray);
            Subtitle[] searchResult;
            try
            {
                _logger.Debug("SubtitleDownloaderWrapper", "Searching with downloader {0}", Name);
                var watch = Stopwatch.StartNew();
                searchResult = _downloader.SearchSubtitles(query).ToArray();
                watch.Stop();
                _logger.Debug("SubtitleDownloaderWrapper", "Done searching with downloader {0} in {1} ms - found {2} candidate(s)", Name, watch.ElapsedMilliseconds, searchResult.Length);
            }
            catch (Exception ex)
            {
                _logger.Verbose("SubtitleDownloaderWrapper", "Downloader search for downloader {0} failed with message: {1}", Name, ex.Message);
                return Enumerable.Empty<Subtitle>();
            }

            return FilterOutSubtitlesNotMatching(tvReleaseIdentity, searchResult);
        }

        private static SearchQuery CreateSearchQuery(TvReleaseIdentity tvReleaseIdentity, Language[] languageArray)
        {
            var query = new SearchQuery(tvReleaseIdentity.SeriesName, tvReleaseIdentity.Season, tvReleaseIdentity.Episode,
                tvReleaseIdentity.ReleaseGroup) { Languages = languageArray };
            query.Enhancements.AddRange(tvReleaseIdentity.Enhancements);
            return query;
        }

        public bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile)
        {
            _logger.Verbose("SubtitleDownloaderWrapper", "Downloading [{1}] subtitles from {0}...", _downloader.GetName(), subtitle.Language.Name);
            try
            {
                var subtitleFile = DownloadSubtitleFile(_downloader, subtitle);
                string targetSubtitleFileName = CreateSubtitleFileName(targetSubtitleFile, subtitle);
                _logger.Debug("SubtitleDownloaderWrapper", "Renaming from {0} to {1}...", subtitleFile, targetSubtitleFileName);
                _fileOperations.RenameSubtitleFile(subtitleFile, targetSubtitleFileName);
                return true;
            }
            catch (Exception e)
            {
                _logger.Verbose("SubtitleDownloaderWrapper", "Downloading from downloader {0} failed: {1}", _downloader.GetName(), e.Message);
            }
            return false;
        }

        private static string DownloadSubtitleFile(ISubtitleDownloader downloader, Subtitle subtitle)
        {
            IEnumerable<FileInfo> subtitleFiles = downloader.SaveSubtitle(subtitle);
            FileInfo subtitleFile = subtitleFiles.First();
            return subtitleFile.FullName;
        }

        private static string CreateSubtitleFileName(string targetSubtitleFile, Subtitle subtitle)
        {
            return FileOperations.CreateSubtitleFileName(targetSubtitleFile, "." + subtitle.Language.TwoLetterIsoName + ".srt");
        }

        private IEnumerable<Subtitle> FilterOutSubtitlesNotMatching(TvReleaseIdentity tvReleaseIdentity, IEnumerable<Subtitle> searchResult)
        {
            var subtitleMatcher = new SubtitleMatcher(_nameParser);
            return subtitleMatcher.FilterOutSubtitlesNotMatching(searchResult, tvReleaseIdentity);
        }

        public bool CanHandleAtLeastOneOf(IEnumerable<Language> languages)
        {
            if (!languages.Any())
                return true;
           return _downloader.SupportedLanguages.Intersect(languages).Any();
        }

        public IEnumerable<IEnhancementRequest> EnhancementRequests => _downloader.EnhancementRequests;
    }
}
