namespace SubtitleFetcher.Common
{
    public interface IDownloadCapabilitiesProvider
    {
        bool CanHandleEpisodeSearchQuery { get; }
        bool CanHandleImdbSearchQuery { get; }
        bool CanHandleSearchQuery { get; }
    }
}