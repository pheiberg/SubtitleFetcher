using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common;

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

        public IEnumerable<Subtitle> SearchSubtitle(EpisodeIdentity episodeIdentity, IEnumerable<string> languages)
        {
            var languageArray = languages.ToArray();
            var query = new SearchQuery(episodeIdentity.SeriesName, episodeIdentity.Season, episodeIdentity.Episode) { LanguageCodes = languageArray };
            IEnumerable<Subtitle> searchResult;
            try
            {
                logger.Debug("EpisodeSubtitleDownloader", "Searching with downloader {0}", Name);
                var watch = Stopwatch.StartNew();
                searchResult = downloader.SearchSubtitles(query);
                watch.Stop();
                logger.Debug("EpisodeSubtitleDownloader", "Done searching with downloader {0} in {1} ms", Name, watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                logger.Verbose("EpisodeSubtitleDownloader", "Downloader search for downloader {0} failed with message: {1}", Name, ex.Message);
                return Enumerable.Empty<Subtitle>();
            }

            var matchingSubtitles = from subtitle in searchResult
                                    let subtitleInfo = nameParser.ParseEpisodeInfo(subtitle.FileName)
                                    let langPriority = Array.FindIndex(languageArray, l => l.Equals(subtitle.LanguageCode))
                                    where subtitleInfo.IsEquivalent(episodeIdentity)
                                    orderby langPriority, subtitleInfo.SeriesName
                                    select subtitle;
            return matchingSubtitles;
        }

        public bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile)
        {
            logger.Verbose("EpisodeSubtitleDownloader", "Downloading [{1}] subtitles from {0}...", downloader.GetName(), subtitle.LanguageCode);
            try
            {
                var subtitleFile = DownloadSubtitleFile(downloader, subtitle);
                string targetSubtitleFileName = FileSystem.CreateSubtitleFileName(targetSubtitleFile, "." + subtitle.LanguageCode + ".srt");
                logger.Debug("EpisodeSubtitleDownloader", "Renaming from {0} to {1}...", subtitleFile, targetSubtitleFileName);
                fileSystem.RenameSubtitleFile(subtitleFile, targetSubtitleFileName);
                return true;
            }
            catch (Exception e)
            {
                logger.Verbose("EpisodeSubtitleDownloader", "Downloading from downloader {0} failed: {1}", downloader.GetName(), e.Message);
            }
            return false;
        }

        public bool CanHandleAtLeastOneOf(IEnumerable<string> languages)
        {
            var extended = downloader as IExtendedSubtitleDownloader;

            if (extended == null)
                return true;

            return extended.LanguageLimitations.Intersect(languages).Any();
        }

        private static string DownloadSubtitleFile(ISubtitleDownloader downloader, Subtitle subtitle)
        {
            IEnumerable<FileInfo> subtitleFiles = downloader.SaveSubtitle(subtitle);
            FileInfo subtitleFile = subtitleFiles.First();
            return subtitleFile.FullName;
        }
    }
}
