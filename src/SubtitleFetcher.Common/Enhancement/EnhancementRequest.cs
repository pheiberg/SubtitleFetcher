using System;

namespace SubtitleFetcher.Common.Enhancement
{
    public class EnhancementRequest<T> : IEnhancementRequest where T : IEnhancement
    {
        public static IEnhancementRequest Make()
        {
            return new EnhancementRequest<T>();
        }

        public Type EnhancementType => typeof (T);
    }
}