using SubtitleFetcher.Common.Hashing;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement
{
    public class OpenSubtitlesHasher : IHexadecimalFileHasher
    {
        private readonly FileHasher _hasher;

        public OpenSubtitlesHasher()
        {
            _hasher = new FileHasher(new OpenSubtitlesHashCalculator());
        }

        public string ComputeHash(string filePath)
        {
            return _hasher.CreateHash(filePath).ToLowerHexString();
        }
    }
}
