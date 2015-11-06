using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Common;

namespace SubtitleFetcher
{
    public class SubtitleDownloadService : ISubtitleDownloadService
    {
        private readonly IEnumerable<IEpisodeSubtitleDownloader> subtitleDownloaders;

        public SubtitleDownloadService(IEnumerable<IEpisodeSubtitleDownloader> subtitleDownloaders)
        {
            this.subtitleDownloaders = subtitleDownloaders;
        }

        public bool DownloadSubtitle(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages)
        {
            var matches = FindMatchingSubtitlesOrderedByLanguageCode(tvReleaseIdentity, languages).ToArray();
           return DownloadFirstAvailableSubtitle(targetSubtitleFile, matches);
        }

        private IEnumerable<DownloaderMatch> FindMatchingSubtitlesOrderedByLanguageCode(TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages)
        {
            var languageArray = languages.ToArray();
            return subtitleDownloaders
                .Where(s => s.CanHandleAtLeastOneOf(languageArray))
                .AsParallel()
                .SelectMany(downloader => downloader.SearchSubtitle(tvReleaseIdentity, languageArray)
                    .Select(match => new DownloaderMatch(downloader, match)))
                .AsSequential()
                .OrderBy(pair => Array.FindIndex(languageArray, arrayItem => arrayItem == pair.Subtitle.LanguageCode))
                .ThenBy(pair => pair.Subtitle.FileName);
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