using CommandLine;

namespace SubtitleFetcher
{
    internal static class ParserExtensions
    {
        public static T ParseArguments<T>(this Parser parser, string[] args) where T : new()
        {
            var options = new T();
            parser.ParseArguments(args, options);
            return options;
        }
    }
}