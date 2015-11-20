using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using SubtitleFetcher.Bootstrapping;
using SubtitleFetcher.Settings;

namespace SubtitleFetcher
{
    public class Program
    {
        private static readonly SubtitleDownloaderCapabilitiesProvider CapabilitiesProvider = new SubtitleDownloaderCapabilitiesProvider();

        public static void Main(string[] args)
        {
            var options = ParseOptions(args);
            HandleParseErrors(options);
            HandleHelpRequests(options);

            var application = InitializeApplication(options);
            application.Run(options);
        }

        private static Options ParseOptions(string[] args)
        {
            var parserSettings = new OptionsParserSettings
            {
                HelpWriter = Console.Error
            };
            var parser = new OptionsParser(parserSettings);
            return parser.ParseOptions(args);
        }

        private static void HandleParseErrors(Options options)
        {
            if(options.ParseErrors.Any())
            {
                Environment.Exit(1);
            }
            if (options.CustomParseErrors.Any())
            {
                PrintCustomErrorInfo(options.CustomParseErrors);
                Environment.Exit(1);
            }
        }

        private static void PrintCustomErrorInfo(IEnumerable<ParseError> customParseErrors)
        {
            Console.Error.WriteLine(CommandLine.Text.HeadingInfo.Default);
            Console.Error.WriteLine(CommandLine.Text.CopyrightInfo.Default);
            Console.Error.WriteLine();
            Console.Error.WriteLine("  ERROR(S): ");
            foreach (var error in customParseErrors)
            {
                Console.Error.WriteLine($"\t{error.Message}");
            }
            Console.Error.WriteLine();
            CapabilitiesProvider.ListAvailableLanguages();
        }

        private static void HandleHelpRequests(Options options)
        {
            if (options.ListDownloaders)
            {
                CapabilitiesProvider.ListAvailableDownloaders();
                Environment.Exit(0);
            }

            if (options.ListLanguages)
            {
                CapabilitiesProvider.ListAvailableLanguages();
                Environment.Exit(0);
            }
        }

        private static DownloaderApplication InitializeApplication(Options options)
        {
            var container = new Container(x => x.AddRegistry(new SubtitleDownloaderRegistry(options)));
            var application = container.GetInstance<DownloaderApplication>();
            return application;
        }
    }
}