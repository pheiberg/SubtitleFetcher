using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleDownloader.Core;
using SubtitleDownloader.Implementations.OpenSubtitles;

namespace SubtitleFetcher
{
    public class SubtitleDownloadService
    {
        private readonly IEnumerable<ISubtitleDownloader> subtitleDownloaders;
        private readonly string language;
        private readonly Logger logger;
        private readonly EpisodeParser episodeParser;

        public SubtitleDownloadService(IEnumerable<ISubtitleDownloader> subtitleDownloaders, string language, Logger logger, EpisodeParser episodeParser)
        {
            this.subtitleDownloaders = subtitleDownloaders;
            this.language = language;
            this.logger = logger;
            this.episodeParser = episodeParser;
        }

        public bool DownloadSubtitle(string targetSubtitleFile, EpisodeIdentity episodeIdentity)
        {
            var query = new EpisodeSearchQuery(episodeIdentity.SeriesName, episodeIdentity.Season, episodeIdentity.Episode, null) { LanguageCodes = new[] { language } };
            return subtitleDownloaders.Any(downloader => TryDownloadFile(targetSubtitleFile, query, downloader, episodeParser, logger, episodeIdentity));
        }

        private bool TryDownloadFile(string targetSubtitleFile, EpisodeSearchQuery query, ISubtitleDownloader downloader, EpisodeParser nameParser, Logger logger, EpisodeIdentity episodeIdentity)
        {
            List<Subtitle> searchResult;
            try
            {
                searchResult = downloader.SearchSubtitles(query);
            }
            catch (Exception e)
            {
                logger.Log("Query to downloader {0} failed: {1}", downloader.GetName(), e.Message);
                return false;
            }

            var matchingSubtitles = from subtitle in searchResult
                                    let subtitleInfo = nameParser.ParseEpisodeInfo(subtitle.FileName)
                                    where subtitleInfo.IsEquivalent(episodeIdentity)
                                    select subtitle;

            foreach (Subtitle subtitle in matchingSubtitles)
            {
                logger.Log("Downloading subtitles from {0}...", downloader.GetName());
                try
                {
                    var subtitleFile = DownloadSubtitleFile(downloader, subtitle);
                    logger.Log("Renaming from {0} to {1}...", subtitleFile.FullName, targetSubtitleFile);
                    RenameSubtitleFile(targetSubtitleFile, subtitleFile.FullName);
                    return true;
                }
                catch (Exception e)
                {
                    logger.Log("Downloading from downloader {0} failed: {1}", downloader.GetName(), e.Message);
                }
            }

            return false;
        }

        private static FileInfo DownloadSubtitleFile(ISubtitleDownloader downloader, Subtitle subtitle)
        {
            List<FileInfo> subtitleFiles = downloader.SaveSubtitle(subtitle);
            FileInfo subtitleFile = subtitleFiles.First();
            return subtitleFile;
        }

        private static void RenameSubtitleFile(string targetSubtitleFile, string sourceFileName)
        {
            File.Delete(targetSubtitleFile);
            File.Move(sourceFileName, targetSubtitleFile);
        }
    }
}