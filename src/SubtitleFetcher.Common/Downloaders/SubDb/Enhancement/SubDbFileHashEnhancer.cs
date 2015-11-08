using System;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Downloaders.SubDb.Enhancement
{
    public class SubDbFileHashEnhancer : IEnhancer
    {
        static readonly SubDbFileHasher Hasher = new SubDbFileHasher();
        
        public IEnhancement Enhance(string filePath, TvReleaseIdentity identity)
        {
            var fileHash = Hasher.ComputeHash(filePath);
            return new SubDbFileHashEnhancement {FileHash = fileHash};
        }

        public Type ProvidedEnhancement => typeof (SubDbFileHashEnhancement);
    }
}
