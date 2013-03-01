using System.Collections.Generic;
using SubtitleDownloader.Core;
using SubtitleFetcher.Common;

namespace SubtitleFetcher
{
    public interface IEpisodeSubtitleDownloader
    {
        IEnumerable<Subtitle> SearchSubtitle(EpisodeIdentity episodeIdentity, IEnumerable<string> languages);

        bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile);
    }
}