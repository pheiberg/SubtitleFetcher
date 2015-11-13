using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Orchestration
{
    public interface ISubtitleRanker
    {
        int GetRankingScore(Subtitle subtitle, Language[] languages, TvReleaseIdentity identity);
    }
}