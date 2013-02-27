using System.Text.RegularExpressions;

namespace TvShowIdentification
{
    public static class StringExtensions
    {
        public static string RemoveNonAlphaNumericChars(this string text)
        {
            return Regex.Replace(text, "[\\W_]", "");
        }
    }
}
