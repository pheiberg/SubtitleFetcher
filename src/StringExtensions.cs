using System.Text.RegularExpressions;

namespace SubtitleFetcher
{
    public static class StringExtensions
    {
        public static string RemoveNonAlphaNumericChars(this string text)
        {
            return Regex.Replace(text, "[\\W_]", "");
        }
    }
}
