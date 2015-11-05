using System.Security.Cryptography;

namespace SubtitleFetcher.Common.Hashing
{
    public class HashAlgorithmFactory<T> : IHashAlgorithmFactory where T : HashAlgorithm, new()
    {
        public HashAlgorithm CreateAlgorithm()
        {
            return new T();
        }
    }
}