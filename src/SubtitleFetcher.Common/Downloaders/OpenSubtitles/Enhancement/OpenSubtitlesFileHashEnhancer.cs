using System;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement
{
    public class OpenSubtitlesFileHashEnhancer : IEnhancer
    {
        private static readonly OpenSubtitlesFileHasher Hasher = new OpenSubtitlesFileHasher();
        
        public IEnhancement Enhance(string filePath, TvReleaseIdentity identity)
        {
            var fileHash = Hasher.ComputeHash(filePath);
            return new OpenSubtitlesFileHashEnhancement {FileHash = fileHash};
        }

        public Type ProvidedEnhancement => typeof(OpenSubtitlesFileHashEnhancement);
    }
}
