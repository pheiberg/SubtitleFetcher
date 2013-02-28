using System;
using System.Text.RegularExpressions;

namespace TvShowIdentification
{
    public static class StringExtensions
    {
        public static string RemoveNonAlphaNumericChars(this string text)
        {
            return Regex.Replace(text, "[\\W_]", "");
        }

        public static string TrimSuffix(this string text, string suffix)
        {
            return text.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase) ? text.Substring(0, text.Length - suffix.Length) : text;
        }
    }
}
