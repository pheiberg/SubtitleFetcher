using System;
using System.Linq;
using CommandLine;
using SubtitleFetcher.Bootstrapping;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Settings
{
    public class OptionsParser
    {
        private readonly OptionsParserSettings _settings;
        private readonly Parser _parser;

        public OptionsParser(OptionsParserSettings settings)
        {
            _settings = settings;
            _parser = new Parser(parserSettings =>
            {
                parserSettings.IgnoreUnknownArguments = false;
                parserSettings.HelpWriter = _settings.HelpWriter;
                parserSettings.EnableDashDash = true;
            });
        }

        public Options ParseOptions(string[] args)
        {
            Options options;
            var results = _parser.ParseArguments<Options>(args);
            if (results.Tag == ParserResultType.Parsed)
            {
                options = ((Parsed<Options>)results).Value;
                ValidateLanguages(options);
                ValidateDownloaders(options);
                return options;
            }

            options = new Options();
            var errors = ((NotParsed<Options>)results).Errors;
            var parseErrors = errors.Select(e => new ParseError(e.Tag.ToString()));
            options.ParseErrors.AddRange(parseErrors);
            return options;
        }

        private static void ValidateLanguages(Options options)
        {
            var knownLanguageCodes = KnownLanguages.AllLanguages.Select(l => l.TwoLetterIsoName);
            var invalidLanguages = options.Languages.Where(language => 
                    !knownLanguageCodes.Contains(language, StringComparer.OrdinalIgnoreCase));
            var parserErrors = invalidLanguages.Select(language => 
                new ParseError($"Invalid language '{language}'"));
            
            options.CustomParseErrors.AddRange(parserErrors);
        }

        private static void ValidateDownloaders(Options options)
        {
            var downloaders = ReflectionHelper.GetAllConcreteImplementors<ISubtitleDownloader>();
            var knownDownloaderNames = downloaders.Select(downloader => downloader.Name.TrimSuffix("Downloader"));
            var invalidDownloaders = options.DownloaderNames.Where(downloader =>
                !knownDownloaderNames.Contains(downloader, StringComparer.OrdinalIgnoreCase));

            var parserErrors = invalidDownloaders.Select(downloader =>
                new ParseError($"Invalid downloader '{downloader}'"));

            options.CustomParseErrors.AddRange(parserErrors);
        }
    }
}