using System.IO;
using SubtitleFetcher.Common.Hashing;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement
{
    public class OpenSubtitlesFileHasher : IHexadecimalFileHasher
    {
        private readonly FileHasher _hasher;

        public OpenSubtitlesFileHasher()
        {
            _hasher = new FileHasher(new OpenSubtitlesHashCalculator());
        }

        public string ComputeHash(string filePath)
        {
            return _hasher.CreateHash(filePath).ToLowerHexString();
        }

        public string ComputeHash(FileInfo file)
        {
            return _hasher.CreateHash(file).ToLowerHexString();
        }
    }
}
