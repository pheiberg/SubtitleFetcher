using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Orchestration
{
    public class SubtitleDownloadService : ISubtitleDownloadService
    {
        private readonly IEnumerable<IEpisodeSubtitleDownloader> _subtitleDownloaders;
        private readonly IEnhancementProvider _enhancementProvider;

        public SubtitleDownloadService(IEnumerable<IEpisodeSubtitleDownloader> subtitleDownloaders, IEnhancementProvider enhancementProvider)
        {
            _subtitleDownloaders = subtitleDownloaders;
            _enhancementProvider = enhancementProvider;
        }

        public bool DownloadSubtitle(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages)
        {
            var matches = FindMatchingSubtitlesOrderedByLanguageCode(targetSubtitleFile, tvReleaseIdentity, languages).ToArray();
           return DownloadFirstAvailableSubtitle(targetSubtitleFile, matches);
        }

        private IEnumerable<DownloaderMatch> FindMatchingSubtitlesOrderedByLanguageCode(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages)
        {
            var languageArray = languages.ToArray();
            var compatibleDownloaders = GetDownloadersForLanguages(languageArray);
            EnhanceIdentity(compatibleDownloaders, tvReleaseIdentity, targetSubtitleFile);
            var searchResults = SearchDownloaders(compatibleDownloaders, tvReleaseIdentity, languageArray);
            return OrderByLanguageCode(searchResults, languageArray);
        }

        private void EnhanceIdentity(IEnumerable<IEpisodeSubtitleDownloader> compatibleDownloaders, TvReleaseIdentity tvReleaseIdentity, string filePath)
        {
            var enhancementRequests = compatibleDownloaders.SelectMany(d => d.EnhancementRequests);
            var enhancements = enhancementRequests.Select(
                    er => _enhancementProvider.GetEnhancement(er.EnhancementType, filePath, tvReleaseIdentity));
            foreach (var enhancement in enhancements)
            {
                tvReleaseIdentity.Enhancements.Add(enhancement);
            }
        }
        
        private IEnumerable<IEpisodeSubtitleDownloader> GetDownloadersForLanguages(string[] languageArray)
        {
            return _subtitleDownloaders.Where(s => s.CanHandleAtLeastOneOf(languageArray));
        }

        private static IEnumerable<DownloaderMatch> SearchDownloaders(IEnumerable<IEpisodeSubtitleDownloader> compatibleDownloaders, TvReleaseIdentity tvReleaseIdentity, string[] languageArray)
        {
            var searchResults = compatibleDownloaders
                .AsParallel()
                .SelectMany(downloader => downloader.SearchSubtitle(tvReleaseIdentity, languageArray)
                    .Select(match => new DownloaderMatch(downloader, match)))
                .AsSequential().ToArray();
            return FilterOutLanguagesNotInRequest(searchResults, languageArray); ;
        }
        private static IEnumerable<DownloaderMatch> FilterOutLanguagesNotInRequest(IEnumerable<DownloaderMatch> searchResults, string[] validLanguages)
        {
            return searchResults.Where(m => validLanguages.Contains(m.Subtitle.LanguageCode));
        }

        private static int FindPreferenceIndexOfLanguage(string[] languageArray, string languageCode)
        {
            return Array.FindIndex(languageArray, arrayItem => arrayItem == languageCode);
        }

        private static IOrderedEnumerable<DownloaderMatch> OrderByLanguageCode(IEnumerable<DownloaderMatch> searchResults, string[] languageArray)
        {
            return searchResults
                .OrderBy(match => FindPreferenceIndexOfLanguage(languageArray, match.Subtitle.LanguageCode))
                .ThenBy(match => match.Subtitle.FileName);
        }

        private static bool DownloadFirstAvailableSubtitle(string targetSubtitleFile, IEnumerable<DownloaderMatch> orderedMatches)
        {
            return orderedMatches.Any(match => match.Downloader.TryDownloadSubtitle(match.Subtitle, targetSubtitleFile));
        }

        private class DownloaderMatch
        {
            public IEpisodeSubtitleDownloader Downloader { get; }
            public Subtitle Subtitle { get; }

            public DownloaderMatch(IEpisodeSubtitleDownloader downloader, Subtitle subtitle)
            {
                Downloader = downloader;
                Subtitle = subtitle;
            }
        }
    }
}