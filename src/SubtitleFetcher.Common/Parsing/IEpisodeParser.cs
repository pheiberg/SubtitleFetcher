namespace SubtitleFetcher.Common.Parsing
{
    public interface IEpisodeParser
    {
        TvReleaseIdentity ParseEpisodeInfo(string fileName);

        TvReleaseIdentity ExtractReleaseIdentity(Subtitle subtitle);
    }
}