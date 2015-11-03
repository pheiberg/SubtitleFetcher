namespace SubtitleFetcher.Common
{
    public interface IEpisodeParser
    {
        TvReleaseIdentity ParseEpisodeInfo(string fileName);
    }
}