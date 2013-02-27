using TvShowIdentification;

namespace SubtitleFetcher
{
    public interface ISubtitleDownloadService
    {
        bool DownloadSubtitle(string targetSubtitleFile, EpisodeIdentity episodeIdentity);
    }
}