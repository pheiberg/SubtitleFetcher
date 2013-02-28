using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SubtitleDownloader.Core;
using TvShowIdentification;

namespace SubtitleFetcher
{
    public class EpisodeSubtitleDownloader : IEpisodeSubtitleDownloader
    {
        private readonly ISubtitleDownloader downloader;
        private readonly IEpisodeParser nameParser;
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;

        public EpisodeSubtitleDownloader(ISubtitleDownloader downloader, IEpisodeParser nameParser, ILogger logger, IFileSystem fileSystem)
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
                logger.Debug("Searching with downloader {0}", Name);
                var watch = Stopwatch.StartNew();
                searchResult = downloader.SearchSubtitles(query);
                watch.Stop();
                logger.Debug("Done searching with downloader {0} in {1} ms", Name, watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                logger.Verbose("Downloader search for downloader {0} failed with message: {1}", Name, ex.Message);
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
            logger.Verbose("Downloading [{1}] subtitles from {0}...", downloader.GetName(), subtitle.LanguageCode);
            try
            {
                var subtitleFile = DownloadSubtitleFile(downloader, subtitle);
                logger.Debug("Renaming from {0} to {1}...", subtitleFile, targetSubtitleFile);
                fileSystem.RenameSubtitleFile(targetSubtitleFile, subtitleFile);
                return true;
            }
            catch (Exception e)
            {
                logger.Verbose("Downloading from downloader {0} failed: {1}", downloader.GetName(), e.Message);
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
