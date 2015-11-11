using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement
{
    public class OpenSubtitlesFileHashEnhancement : IEnhancement
    {
        public string FileHash { get; set; }

        public long FileByteSize { get; set; }
    }
}