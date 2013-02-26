using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var matches = subtitleDownloaders.AsParallel()
                    .SelectMany(downloader => downloader.SearchSubtitle(episodeIdentity, languages)
                    .Select(match => new DownloaderMatch(downloader, match)))
                    .OrderBy(pair => Array.FindIndex(languages, arrayItem => arrayItem == pair.Subtitle.LanguageCode))
                    .ThenBy(pair => pair.Subtitle.FileName);
            
           return DownloadFirstAvailableSubtitle(targetSubtitleFile, matches);
        }

        private static bool DownloadFirstAvailableSubtitle(string targetSubtitleFile, IEnumerable<DownloaderMatch> orderedMatches)
        {
            return orderedMatches.Any(match => match.Downloader.TryDownloadSubtitle(match.Subtitle, targetSubtitleFile));
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