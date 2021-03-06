﻿using System;
using System.IO;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement
{
    public class OpenSubtitlesFileHashEnhancer : IEnhancer
    {
        private static readonly OpenSubtitlesFileHasher Hasher = new OpenSubtitlesFileHasher();
        
        public IEnhancement Enhance(string filePath, TvReleaseIdentity identity)
        {
            var byteSize = new FileInfo(filePath).Length;
            var fileHash = Hasher.ComputeHash(filePath);
            if (string.IsNullOrEmpty(fileHash))
                return null;

            return new OpenSubtitlesFileHashEnhancement
            {
                FileByteSize = byteSize,
                FileHash = fileHash
            };
        }

        public Type ProvidedEnhancement => typeof(OpenSubtitlesFileHashEnhancement);
    }
}
