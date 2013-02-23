namespace SubtitleFetcher
{
    public interface IEpisodeParser
    {
        EpisodeIdentity ParseEpisodeInfo(string fileName);
    }
}