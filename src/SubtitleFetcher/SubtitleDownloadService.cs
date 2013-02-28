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
        private readonly string[] languages;

        public SubtitleDownloadService(IEnumerable<IEpisodeSubtitleDownloader> subtitleDownloaders, LanguageSettings languageSettings)
        {
            this.subtitleDownloaders = subtitleDownloaders;
            languages = languageSettings.Languages;
        }

        public bool DownloadSubtitle(string targetSubtitleFile, EpisodeIdentity episodeIdentity)
        {
            var matches = FindMatchingSubtitlesOrderedByLanguageCode(episodeIdentity);
           return DownloadFirstAvailableSubtitle(targetSubtitleFile, matches);
        }

        private IEnumerable<DownloaderMatch> FindMatchingSubtitlesOrderedByLanguageCode(EpisodeIdentity episodeIdentity)
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