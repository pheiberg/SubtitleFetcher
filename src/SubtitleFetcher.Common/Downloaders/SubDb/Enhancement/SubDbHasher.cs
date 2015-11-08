﻿using SubtitleFetcher.Common.Hashing;

namespace SubtitleFetcher.Common.Downloaders.SubDb.Enhancement
{
    public class SubDbHasher : IHexadecimalFileHasher
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
