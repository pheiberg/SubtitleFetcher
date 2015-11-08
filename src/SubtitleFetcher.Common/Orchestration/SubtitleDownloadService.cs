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

        public bool DownloadSubtitle(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<Language> languages)
        {
            var matches = FindMatchingSubtitlesOrderedByLanguageCode(targetSubtitleFile, tvReleaseIdentity, languages).ToArray();
           return DownloadFirstAvailableSubtitle(targetSubtitleFile, matches);
        }

        private IEnumerable<DownloaderMatch> FindMatchingSubtitlesOrderedByLanguageCode(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<Language> languages)
        {
            var languageArray = languages.ToArray();
            var compatibleDownloaders = GetDownloadersForLanguages(languageArray);
            EnhanceIdentity(compatibleDownloaders, tvReleaseIdentity, targetSubtitleFile);
            var searchResults = SearchDownloaders(compatibleDownloaders, tvReleaseIdentity, languageArray);
            return OrderByLanguageCode(searchResults, languageArray);
        }

        private void EnhanceIdentity(IEnumerable<IEpisodeSubtitleDownloader> compatibleDownloaders, TvReleaseIdentity tvReleaseIdentity, string filePath)
        {
            var applicator = new EnhancementApplicator(compatibleDownloaders, _enhancementProvider);
            applicator.ApplyEnhancements(filePath, tvReleaseIdentity);
        }
        
        private IEnumerable<IEpisodeSubtitleDownloader> GetDownloadersForLanguages(Language[] languageArray)
        {
            return _subtitleDownloaders.Where(s => s.CanHandleAtLeastOneOf(languageArray)).ToArray();
        }

        private static IEnumerable<DownloaderMatch> SearchDownloaders(IEnumerable<IEpisodeSubtitleDownloader> compatibleDownloaders, TvReleaseIdentity tvReleaseIdentity, Language[] languageArray)
        {
            var searchResults = compatibleDownloaders
                .AsParallel()
                .SelectMany(downloader => downloader.SearchSubtitle(tvReleaseIdentity, languageArray)
                    .Select(match => new DownloaderMatch(downloader, match)))
                .AsSequential().ToArray();
            return FilterOutLanguagesNotInRequest(searchResults, languageArray); ;
        }
        private static IEnumerable<DownloaderMatch> FilterOutLanguagesNotInRequest(IEnumerable<DownloaderMatch> searchResults, Language[] validLanguages)
        {
            return searchResults.Where(m => validLanguages.Any(l => l == m.Subtitle.Language));
        }

        private static int FindPreferenceIndexOfLanguage(Language[] languageArray, Language language)
        {
            return Array.FindIndex(languageArray, arrayItem => arrayItem == language);
        }

        private static IOrderedEnumerable<DownloaderMatch> OrderByLanguageCode(IEnumerable<DownloaderMatch> searchResults, Language[] languageArray)
        {
            return searchResults
                .OrderBy(match => FindPreferenceIndexOfLanguage(languageArray, match.Subtitle.Language))
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

    public class EnhancementApplicator
    {
        private readonly IEnumerable<IEpisodeSubtitleDownloader> _downloaders;
        private readonly IEnhancementProvider _enhancementProvider;

        public EnhancementApplicator(IEnumerable<IEpisodeSubtitleDownloader>  downloaders, IEnhancementProvider enhancementProvider)
        {
            _downloaders = downloaders;
            _enhancementProvider = enhancementProvider;
        }

        public void ApplyEnhancements(string filePath, TvReleaseIdentity identity)
        {
            var enhancementRequests = _downloaders.SelectMany(d => d.EnhancementRequests);
            var enhancements = enhancementRequests.Select(
                    er => _enhancementProvider.GetEnhancement(er.EnhancementType, filePath, identity));
            foreach (var enhancement in enhancements.Where(enhancement => enhancement != null))
            {
                identity.Enhancements.Add(enhancement);
            }
        }
    }
}