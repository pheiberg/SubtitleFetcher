using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitleFetcher.Common.Orchestration
{
    public class SubtitleDownloadService : ISubtitleDownloadService
    {
        private readonly IEnumerable<IEpisodeSubtitleDownloader> _subtitleDownloaders;

        public SubtitleDownloadService(IEnumerable<IEpisodeSubtitleDownloader> subtitleDownloaders)
        {
            _subtitleDownloaders = subtitleDownloaders;
        }

        public bool DownloadSubtitle(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages)
        {
            var matches = FindMatchingSubtitlesOrderedByLanguageCode(tvReleaseIdentity, languages).ToArray();
           return DownloadFirstAvailableSubtitle(targetSubtitleFile, matches);
        }

        private IEnumerable<DownloaderMatch> FindMatchingSubtitlesOrderedByLanguageCode(TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages)
        {
            var languageArray = languages.ToArray();
            var compatibleDownloaders = GetDownloadersForLanguages(languageArray);
            var searchResults = SearchDownloaders(compatibleDownloaders, tvReleaseIdentity, languageArray);
            return OrderByLanguageCode(searchResults, languageArray);
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