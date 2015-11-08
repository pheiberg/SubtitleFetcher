using System;
using System.Security.Cryptography;

namespace SubtitleFetcher.Common.Hashing
{
    public class HashCalculator<T> : IHashCalculator where T : HashAlgorithm, new()
    {
        public virtual byte[] ComputeHash(byte[] buffer)
        {
            using (var hashAlgorithm = new T())
            {
                return hashAlgorithm.ComputeHash(buffer);
            }
        }
    }
}