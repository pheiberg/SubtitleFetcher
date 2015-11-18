using CommandLine;
using SubtitleFetcher.Common;

namespace SubtitleFetcher.Settings
{
    public class OptionsParser
    {
        private readonly OptionsParserSettings _settings;

        public OptionsParser(OptionsParserSettings settings)
        {
            _settings = settings;
        }

        public Options ParseOptions(string[] args)
        {
            var parser = new Parser(settings =>
            {
                settings.IgnoreUnknownArguments = false;
                settings.HelpWriter = _settings.HelpWriter;
                settings.EnableDashDash = true;
            });
            var results = parser.ParseArguments<Options>(args);
            if (results.Tag == ParserResultType.Parsed)
                return ((Parsed<Options>)results).Value;

            var options = new Options();
            options.ParseErrors.AddRange(((NotParsed<Options>)results).Errors);
            return options;
        }
    }
}