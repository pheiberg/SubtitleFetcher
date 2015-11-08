using System;
using SubtitleFetcher.Common.Hashing;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles.Enhancement
{
    public class OpenSubtitlesHashCalculator : IHashCalculator
    {
        public byte[] ComputeHash(byte[] buffer)
        {
            ulong lhash = 0;
            for (int i = 0; i < 2 * 65536; i += 8)
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
