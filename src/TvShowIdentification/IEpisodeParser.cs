namespace TvShowIdentification
{
    public interface IEpisodeParser
    {
        EpisodeIdentity ParseEpisodeInfo(string fileName);
    }
}