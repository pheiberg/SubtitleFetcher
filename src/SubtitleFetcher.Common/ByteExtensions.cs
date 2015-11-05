using System;

namespace SubtitleFetcher.Common
{
    public static class ByteExtensions
    {
        static readonly char[] HexDigits = "0123456789ABCDEF".ToCharArray();
        
        public static string ToUpperHexString(this byte[] bytes)
        {
            var digits = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                int digit2;
                var digit1 = Math.DivRem(bytes[i], 16, out digit2);
                digits[2 * i] = HexDigits[digit1];
                digits[2 * i + 1] = HexDigits[digit2];
            }
            return new string(digits);
        }
    }
}