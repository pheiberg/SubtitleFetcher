using System.Collections.Generic;
using SubtitleFetcher.Common;

namespace SubtitleFetcher
{
    public interface ISubtitleDownloadService
    {
        bool DownloadSubtitle(string targetSubtitleFile, EpisodeIdentity episodeIdentity, IEnumerable<string> languages);
    }
}