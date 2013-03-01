using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleDownloader.Core;
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

        public bool DownloadSubtitle(string targetSubtitleFile, EpisodeIdentity episodeIdentity, string[] languages)
        {
            var matches = FindMatchingSubtitlesOrderedByLanguageCode(episodeIdentity, languages);
           return DownloadFirstAvailableSubtitle(targetSubtitleFile, matches);
        }

        private IEnumerable<DownloaderMatch> FindMatchingSubtitlesOrderedByLanguageCode(EpisodeIdentity episodeIdentity, string[] languages)
        {
            return subtitleDownloaders.AsParallel()
                .SelectMany(downloader => downloader.SearchSubtitle(episodeIdentity, languages)
                    .Select(match => new DownloaderMatch(downloader, match)))
                .OrderBy(pair => Array.FindIndex(languages, arrayItem => arrayItem == pair.Subtitle.LanguageCode))
                .ThenBy(pair => pair.Subtitle.FileName);
        }

        private static bool DownloadFirstAvailableSubtitle(string targetSubtitleFile, IEnumerable<DownloaderMatch> orderedMatches)
        {
            return orderedMatches.Any(match => match.Downloader.TryDownloadSubtitle(match.Subtitle, targetSubtitleFile));
        }

        private class DownloaderMatch
        {
            public IEpisodeSubtitleDownloader Downloader { get; private set; }
            public Subtitle Subtitle { get; private set; }

            public DownloaderMatch(IEpisodeSubtitleDownloader downloader, Subtitle subtitle)
            {
                Downloader = downloader;
                Subtitle = subtitle;
            }
        }
    }
}