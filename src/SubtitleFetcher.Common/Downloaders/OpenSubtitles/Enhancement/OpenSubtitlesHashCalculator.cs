using System;
using SubtitleFetcher.Common.Hashing;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement
{
    public class OpenSubtitlesHashCalculator : IHashCalculator
    {
        private const int BlockSize = 64*1024;

        public byte[] ComputeHash(byte[] buffer)
        {
            if (buffer.Length < BlockSize)
                return new byte[0];

            ulong lhash = 0;
            for (int i = 0; i < 2 * BlockSize; i += 8)
            {
                unchecked
                {
                    lhash += BitConverter.ToUInt64(buffer, i);
                }
            }
            byte[] result = BitConverter.GetBytes(lhash);
            Array.Reverse(result);
            return result;
        }
    }
}
