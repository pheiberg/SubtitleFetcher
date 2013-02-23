using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleDownloader.Core;

namespace SubtitleFetcher
{
    public class SubtitleDownloadService : ISubtitleDownloadService
    {
        private readonly IEnumerable<ISubtitleDownloadProvider> subtitleDownloaders;
        private readonly string[] languages;

        public SubtitleDownloadService(IEnumerable<ISubtitleDownloadProvider> subtitleDownloaders, string[] languages)
        {
            this.subtitleDownloaders = subtitleDownloaders;
            this.languages = languages;
        }

        public bool DownloadSubtitle(string targetSubtitleFile, EpisodeIdentity episodeIdentity)
        {
            var matches = new List<DownloaderMatch>();
            foreach(var downloadProvider in subtitleDownloaders)
            {
                var downloader = downloadProvider;
                var subtitles = downloader.SearchSubtitle(episodeIdentity, languages);
                matches.AddRange(subtitles.Select(subtitle => new DownloaderMatch(downloader, subtitle)));
            }

            var orderedMatches =
                from match in matches
                orderby Array.FindIndex(languages, l => l == match.Subtitle.LanguageCode) , match.Subtitle.FileName
                select match;

            return DownloadFirstAvailableSubtitle(targetSubtitleFile, orderedMatches);
        }

        private static bool DownloadFirstAvailableSubtitle(string targetSubtitleFile, IEnumerable<DownloaderMatch> orderedMatches)
        {
            return orderedMatches.Any(match =>  match.Downloader.TryDownloadSubtitle(match.Subtitle, targetSubtitleFile));
        }

        private class DownloaderMatch
        {
            public ISubtitleDownloadProvider Downloader { get; private set; }
            public Subtitle Subtitle { get; private set; }

            public DownloaderMatch(ISubtitleDownloadProvider downloader, Subtitle subtitle)
            {
                Downloader = downloader;
                Subtitle = subtitle;
            }
        }
    }
}