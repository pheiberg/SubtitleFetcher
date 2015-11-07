using System;

namespace SubtitleFetcher.Common.Enhancement
{
    public interface IEnhancer
    {
        IEnhancement Enhance(string filePath, TvReleaseIdentity identity);

        Type ProvidedEnhancement { get; }
    }
}