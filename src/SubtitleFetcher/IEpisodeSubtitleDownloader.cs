using System.Collections.Generic;
using SubtitleDownloader.Core;
using TvShowIdentification;

namespace SubtitleFetcher
{
    public interface ISubtitleDownloadProvider
    {
        IEnumerable<Subtitle> SearchSubtitle(EpisodeIdentity episodeIdentity, string[] languages);

        bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile);
    }
}