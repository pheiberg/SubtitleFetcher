using System.Security.Cryptography;

namespace SubtitleFetcher.Common.Hashing
{
    public interface IHashAlgorithmFactory
    {
        HashAlgorithm CreateAlgorithm();
    }
}