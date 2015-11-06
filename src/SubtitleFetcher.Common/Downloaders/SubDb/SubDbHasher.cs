using System.IO;
using SubtitleFetcher.Common.Hashing;

namespace SubtitleFetcher.Common.Downloaders.SubDb
{
    public class SubDbHasher
    {
        private readonly FileHasher _hasher;

        public SubDbHasher()
        {
            _hasher = new FileHasher(new HashCalculator(new Md5HashAlgorithmFactory()));
        }

        public string ComputeHash(string filePath)
        {
            return _hasher.CreateHash(filePath).ToLowerHexString();
        }
    }
}
