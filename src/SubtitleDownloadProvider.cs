using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleDownloader.Core;

namespace SubtitleFetcher
{
    public class SubtitleDownloadProvider : ISubtitleDownloadProvider
    {
        private readonly ISubtitleDownloader downloader;
        private readonly IEpisodeParser nameParser;
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;

        public SubtitleDownloadProvider(ISubtitleDownloader downloader, IEpisodeParser nameParser, ILogger logger, IFileSystem fileSystem)
        {
            this.downloader = downloader;
            this.nameParser = nameParser;
            this.logger = logger;
            this.fileSystem = fileSystem;
        }

        public string Name
        {
            get { return downloader.GetName(); }
        }

        public IEnumerable<Subtitle> SearchSubtitle(EpisodeIdentity episodeIdentity, string[] languages)
        {
            var query = new EpisodeSearchQuery(episodeIdentity.SeriesName, episodeIdentity.Season, episodeIdentity.Episode, null) { LanguageCodes = languages };
            IEnumerable<Subtitle> searchResult;
            try
            {
                searchResult = downloader.SearchSubtitles(query);
            }
            catch (Exception ex)
            {
                logger.Log("Dowloader search for downloader {0} failed with message: {1}", Name, ex.Message);
                return Enumerable.Empty<Subtitle>();
            }

            var matchingSubtitles = from subtitle in searchResult
                                    let subtitleInfo = nameParser.ParseEpisodeInfo(subtitle.FileName)
                                    let langPriority = Array.FindIndex(languages, l => l.Equals(subtitle.LanguageCode))
                                    where subtitleInfo.IsEquivalent(episodeIdentity)
                                    orderby langPriority, subtitleInfo.SeriesName
                                    select subtitle;
            return matchingSubtitles;
        }

        public bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile)
        {
            logger.Log("Downloading [{1}] subtitles from {0}...", downloader.GetName(), subtitle.LanguageCode);
            try
            {
                var subtitleFile = DownloadSubtitleFile(downloader, subtitle);
                logger.Log("Renaming from {0} to {1}...", subtitleFile, targetSubtitleFile);
                fileSystem.RenameSubtitleFile(targetSubtitleFile, subtitleFile);
                return true;
            }
            catch (Exception e)
            {
                logger.Log("Downloading from downloader {0} failed: {1}", downloader.GetName(), e.Message);
            }
            return false;
        }

        private static string DownloadSubtitleFile(ISubtitleDownloader downloader, Subtitle subtitle)
        {
            List<FileInfo> subtitleFiles = downloader.SaveSubtitle(subtitle);
            FileInfo subtitleFile = subtitleFiles.First();
            return subtitleFile.FullName;
        }

    }
}
