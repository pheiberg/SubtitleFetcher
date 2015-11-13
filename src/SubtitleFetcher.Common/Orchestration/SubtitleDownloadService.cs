using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Orchestration
{
    public class SubtitleDownloadService : ISubtitleDownloadService
    {
        private readonly IEnumerable<IEpisodeSubtitleDownloader> _subtitleDownloaders;
        private readonly IEnhancementProvider _enhancementProvider;
        private readonly ISubtitleRanker _subtitleRanker;

        public SubtitleDownloadService(IEnumerable<IEpisodeSubtitleDownloader> subtitleDownloaders, IEnhancementProvider enhancementProvider, ISubtitleRanker subtitleRanker)
        {
            _subtitleDownloaders = subtitleDownloaders;
            _enhancementProvider = enhancementProvider;
            _subtitleRanker = subtitleRanker;
        }

        public bool DownloadSubtitle(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<Language> languages)
        {
           var matches = FindMatchingSubtitlesOrderedByRanking(targetSubtitleFile, tvReleaseIdentity, languages).ToArray();
           return DownloadFirstAvailableSubtitle(targetSubtitleFile, matches);
        }

        private IOrderedEnumerable<DownloaderMatch> FindMatchingSubtitlesOrderedByRanking(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<Language> languages)
        {
            var languageArray = languages.ToArray();
            var compatibleDownloaders = GetDownloadersForLanguages(languageArray);
            EnhanceIdentity(compatibleDownloaders, tvReleaseIdentity, targetSubtitleFile);
            var searchResults = SearchDownloaders(compatibleDownloaders, languageArray, tvReleaseIdentity);
            return OrderByRanking(searchResults, languageArray, tvReleaseIdentity);
        }

        private void EnhanceIdentity(IEnumerable<IEpisodeSubtitleDownloader> compatibleDownloaders, TvReleaseIdentity tvReleaseIdentity, string filePath)
        {
            var applicator = new EnhancementApplicator(compatibleDownloaders, _enhancementProvider);
            applicator.ApplyEnhancements(filePath, tvReleaseIdentity);
        }
        
        private IEpisodeSubtitleDownloader[] GetDownloadersForLanguages(Language[] languageArray)
        {
            return _subtitleDownloaders.Where(s => s.CanHandleAtLeastOneOf(languageArray)).ToArray();
        }

        private static IEnumerable<DownloaderMatch> SearchDownloaders(IEnumerable<IEpisodeSubtitleDownloader> compatibleDownloaders, Language[] languageArray, TvReleaseIdentity tvReleaseIdentity)
        {
            var searchResults = compatibleDownloaders
                .AsParallel()
                .SelectMany(downloader => downloader.SearchSubtitle(tvReleaseIdentity, languageArray)
                    .Select(match => new DownloaderMatch(downloader, match)))
                .AsSequential().ToArray();
            return FilterOutLanguagesNotInRequest(searchResults, languageArray);
        }
        private static IEnumerable<DownloaderMatch> FilterOutLanguagesNotInRequest(IEnumerable<DownloaderMatch> searchResults, Language[] validLanguages)
        {
            return searchResults.Where(m => validLanguages.Any(l => l == m.Subtitle.Language));
        }

        private IOrderedEnumerable<DownloaderMatch> OrderByRanking(IEnumerable<DownloaderMatch> searchResults, Language[] languageArray, TvReleaseIdentity identity)
        {
            return searchResults.OrderByDescending(s => _subtitleRanker.GetRankingScore(s.Subtitle, languageArray, identity));
        }
        
        private static bool DownloadFirstAvailableSubtitle(string targetSubtitleFile, IEnumerable<DownloaderMatch> orderedMatches)
        {
            foreach (var match in orderedMatches)
            {
                if (match.Downloader.TryDownloadSubtitle(match.Subtitle, targetSubtitleFile))
                {
                    return true;
                }
            }
            return false;
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