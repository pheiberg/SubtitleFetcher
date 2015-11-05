using System;

namespace SubtitleFetcher.Common.Hashing
{
    public class HashCalculator : IHashCalculator
    {
        private readonly IHashAlgorithmFactory _hashAlgorithmFactory;

        public HashCalculator(IHashAlgorithmFactory hashAlgorithmFactory)
        {
            if (hashAlgorithmFactory == null) throw new ArgumentNullException(nameof(hashAlgorithmFactory));
            _hashAlgorithmFactory = hashAlgorithmFactory;
        }

        public virtual byte[] ComputeHash(byte[] buffer)
        {
            using (var hashAlgorithm = _hashAlgorithmFactory.CreateAlgorithm())
            {
                return hashAlgorithm.ComputeHash(buffer);
            }
        }
    }
}