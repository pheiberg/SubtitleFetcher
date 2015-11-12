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
    public class EpisodeSubtitleDownloader : IEpisodeSubtitleDownloader
    {
        private readonly ISubtitleDownloader _downloader;
        private readonly IEpisodeParser _nameParser;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public EpisodeSubtitleDownloader(ISubtitleDownloader downloader, IEpisodeParser nameParser, ILogger logger, IFileSystem fileSystem)
        {
            _downloader = downloader;
            _nameParser = nameParser;
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public string Name => _downloader.GetName();

        public IEnumerable<Subtitle> SearchSubtitle(TvReleaseIdentity tvReleaseIdentity, IEnumerable<Language> languages)
        {
            var languageArray = languages.ToArray();
            var query = CreateSearchQuery(tvReleaseIdentity, languageArray);
            IEnumerable<Subtitle> searchResult;
            try
            {
                _logger.Debug("EpisodeSubtitleDownloader", "Searching with downloader {0}", Name);
                var watch = Stopwatch.StartNew();
                searchResult = _downloader.SearchSubtitles(query);
                watch.Stop();
                _logger.Debug("EpisodeSubtitleDownloader", "Done searching with downloader {0} in {1} ms", Name, watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.Verbose("EpisodeSubtitleDownloader", "Downloader search for downloader {0} failed with message: {1}", Name, ex.Message);
                return Enumerable.Empty<Subtitle>();
            }

            var matchingSubtitles = (from subtitle in searchResult
                                    let subtitleInfo = _nameParser.ParseEpisodeInfo(subtitle.FileName)
                                    let langPriority = Array.FindIndex(languageArray, l => l.Equals(subtitle.Language))
                                    where subtitleInfo.IsEquivalent(tvReleaseIdentity)
                                    orderby langPriority, subtitleInfo.SeriesName
                                    select subtitle).ToArray();
            return matchingSubtitles;
        }

        private static SearchQuery CreateSearchQuery(TvReleaseIdentity tvReleaseIdentity, Language[] languageArray)
        {
            var query = new SearchQuery(tvReleaseIdentity.SeriesName, tvReleaseIdentity.Season, tvReleaseIdentity.Episode,
                tvReleaseIdentity.ReleaseGroup) { Languages = languageArray };
            foreach (var enhancement in tvReleaseIdentity.Enhancements)
            {
                query.Enhancements.Add(enhancement);
            }
            return query;
        }

        public bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile)
        {
            _logger.Verbose("EpisodeSubtitleDownloader", "Downloading [{1}] subtitles from {0}...", _downloader.GetName(), subtitle.Language.Name);
            try
            {
                var subtitleFile = DownloadSubtitleFile(_downloader, subtitle);
                string targetSubtitleFileName = FileSystem.CreateSubtitleFileName(targetSubtitleFile, "." + subtitle.Language.TwoLetterIsoName + ".srt");
                _logger.Debug("EpisodeSubtitleDownloader", "Renaming from {0} to {1}...", subtitleFile, targetSubtitleFileName);
                _fileSystem.RenameSubtitleFile(subtitleFile, targetSubtitleFileName);
                return true;
            }
            catch (Exception e)
            {
                _logger.Verbose("EpisodeSubtitleDownloader", "Downloading from downloader {0} failed: {1}", _downloader.GetName(), e.Message);
            }
            return false;
        }
        
        private static string DownloadSubtitleFile(ISubtitleDownloader downloader, Subtitle subtitle)
        {
            IEnumerable<FileInfo> subtitleFiles = downloader.SaveSubtitle(subtitle);
            FileInfo subtitleFile = subtitleFiles.First();
            return subtitleFile.FullName;
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
