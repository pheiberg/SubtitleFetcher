using System.Security.Cryptography;
using SubtitleFetcher.Common.Hashing;

namespace SubtitleFetcher.Common.Downloaders.SubDb.Enhancement
{
    public class SubDbFileHasher : IHexadecimalFileHasher
    {
        private readonly FileHasher _hasher;

        public SubDbFileHasher()
        {
            _hasher = new FileHasher(new HashCalculator<MD5CryptoServiceProvider>());
        }

        public string ComputeHash(string filePath)
        {
            return _hasher.CreateHash(filePath).ToLowerHexString();
        }
    }
}
