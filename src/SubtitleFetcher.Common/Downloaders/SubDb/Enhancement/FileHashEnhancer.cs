using System;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Downloaders.SubDb.Enhancement
{
    public class FileHashEnhancer : IEnhancer
    {
        static readonly SubDbHasher Hasher = new SubDbHasher();
        
        public IEnhancement Enhance(string filePath, TvReleaseIdentity identity)
        {
            var fileHash = Hasher.ComputeHash(filePath);
            return new FileHashEnhancement {FileHash = fileHash};
        }

        public Type ProvidedEnhancement => typeof (FileHashEnhancement);
    }
}
