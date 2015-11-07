using System;

namespace SubtitleFetcher.Common.Enhancement
{
    public interface IEnhancementProvider
    {
        IEnhancement GetEnhancement<T>(string filePath, TvReleaseIdentity identity) where T : IEnhancement;

        IEnhancement GetEnhancement(Type enhancementType, string filePath, TvReleaseIdentity identity);
    }
}