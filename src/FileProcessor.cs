using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleDownloader.Core;

namespace SubtitleFetcher
{
    public class FileProcessor
    {
        private readonly EpisodeParser episodeParser;
        private static readonly string[] AcceptedExtensions = new[] { ".avi", ".mkv", ".mp4" };
        private readonly IEnumerable<ISubtitleDownloader> subtitleDownloaders;
        private readonly Logger logger;
        private readonly IEnumerable<string> ignoredShows;
        private readonly Options options;

        public FileProcessor(EpisodeParser episodeParser, IEnumerable<ISubtitleDownloader> subtitleDownloaders, Logger logger, IEnumerable<string> ignoredShows, Options options)
        {
            this.episodeParser = episodeParser;
            this.subtitleDownloaders = subtitleDownloaders;
            this.logger = logger;
            this.ignoredShows = ignoredShows;
            this.options = options;
        }

        public bool ProcessFile(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (!AcceptedExtensions.Contains(ext))
                return true;

            var path = Path.GetDirectoryName(fileName);
            var file = Path.GetFileNameWithoutExtension(fileName);
            var targetLocation = Path.Combine(path, file);
            string targetSubtitleFile = targetLocation + ".srt";
            string targetNoSubtitleFile = targetLocation + ".nosrt";
            if (File.Exists(targetSubtitleFile) || File.Exists(targetNoSubtitleFile))
                return true;

            var episodeIdentity = episodeParser.ParseEpisodeInfo(Path.GetFileNameWithoutExtension(fileName));
            if (string.IsNullOrEmpty(episodeIdentity.SeriesName))
                return false;

            if (ignoredShows.Any(s => string.Equals(s, episodeIdentity.SeriesName)))
            {
                logger.Log("Ignoring {0}", fileName);
                return true;
            }

            logger.Log("Processing file {0}...", fileName);

            var query = new EpisodeSearchQuery(episodeIdentity.SeriesName, episodeIdentity.Season, episodeIdentity.Episode, null) { LanguageCodes = new[] { options.Language } };

            return subtitleDownloaders.Any(downloader => TryDownloadFile(targetSubtitleFile, query, downloader, episodeParser, logger, episodeIdentity));
        }

        private bool TryDownloadFile(string targetSubtitleFile, EpisodeSearchQuery query, ISubtitleDownloader downloader, EpisodeParser nameParser, Logger logger, EpisodeIdentity episodeIdentity)
        {
            try
            {
                var searchSubtitles = downloader.SearchSubtitles(query);
                foreach (Subtitle subtitle in searchSubtitles)
                {
                    var subtitleInfo = nameParser.ParseEpisodeInfo(subtitle.FileName);
                    if (!subtitleInfo.IsEquivalent(episodeIdentity))
                        continue;

                    logger.Log("Downloading subtitles from {0}...", downloader.GetName());
                    try
                    {
                        List<FileInfo> subtitleFiles = downloader.SaveSubtitle(subtitle);
                        FileInfo subtitleFile = subtitleFiles.First();
                        logger.Log("Renaming from {0} to {1}...", subtitleFile.FullName, targetSubtitleFile);
                        File.Delete(targetSubtitleFile);
                        File.Move(subtitleFile.FullName, targetSubtitleFile);
                        return true;
                    }
                    catch (Exception e)
                    {
                        logger.Log("Downloader {0} failed: {1}", downloader.GetName(), e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Log("Downloader {0} failed: {1}", downloader.GetName(), e.Message);
            }
            return false;
        }

    }
}