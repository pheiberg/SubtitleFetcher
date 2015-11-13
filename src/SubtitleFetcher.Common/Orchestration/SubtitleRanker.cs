using System;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Orchestration
{
    public class SubtitleRanker : ISubtitleRanker
    {
        public int GetRankingScore(Subtitle subtitle, Language[] languages, TvReleaseIdentity identity)
        {
            int baseScore = 100 - FindPreferenceIndexOfLanguage(languages, subtitle.Language);
            return baseScore;
        }

        private static int FindPreferenceIndexOfLanguage(Language[] languageArray, Language language)
        {
            return Array.FindIndex(languageArray, arrayItem => arrayItem == language);
        }
    }
}