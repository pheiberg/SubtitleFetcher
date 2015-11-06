using System;
using System.Collections.Generic;

namespace SubtitleFetcher.Common
{
    public static class ByteExtensions
    {
        private static string HexString = "0123456789ABCDEF";
        static readonly char[] UpperHexDigits = HexString.ToCharArray();
        static readonly char[] LowerHexDigits = HexString.ToLower().ToCharArray();
        
        public static string ToUpperHexString(this byte[] bytes)
        {
            return ToHexString(bytes, UpperHexDigits);
        }

        public static string ToLowerHexString(this byte[] bytes)
        {
            return ToHexString(bytes, LowerHexDigits);
        }

        private static string ToHexString(IReadOnlyList<byte> bytes, IReadOnlyList<char> hexDigitsLookup)
        {
            var digits = new char[bytes.Count * 2];
            for (var i = 0; i < bytes.Count; i++)
            {
                int digit2;
                var digit1 = Math.DivRem(bytes[i], 16, out digit2);
                digits[2 * i] = hexDigitsLookup[digit1];
                digits[2 * i + 1] = hexDigitsLookup[digit2];
            }
            return new string(digits);
        }
    }
}