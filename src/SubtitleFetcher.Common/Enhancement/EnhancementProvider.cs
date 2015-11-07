using System;

namespace SubtitleFetcher.Common.Enhancement
{
    public class EnhancementProvider : IEnhancementProvider
    {
        private readonly EnhancerRegistry _registry;

        public EnhancementProvider(EnhancerRegistry registry)
        {
            _registry = registry;
        }

        public IEnhancement GetEnhancement<T>(string filePath, TvReleaseIdentity identity) where T : IEnhancement
        {
            return GetEnhancement(typeof (T), filePath, identity);
        }

        public IEnhancement GetEnhancement(Type enhancementType, string filePath, TvReleaseIdentity identity)
        {
            var enhancer = _registry.GetEnhancerFor(enhancementType);
            return enhancer?.Enhance(filePath, identity);
        }
    }
}