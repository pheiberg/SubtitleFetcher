namespace SubtitleFetcher.Common
{
    public interface IEpisodeParser
    {
        EpisodeIdentity ParseEpisodeInfo(string fileName);
    }
}