using System;

namespace SubtitleFetcher.Common.Enhancement
{
    public interface IEnhancementRequest
    {
        Type EnhancementType { get; }
    }
}