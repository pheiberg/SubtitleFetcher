using System;
using System.Linq;
using CommandLine;
using StructureMap;
using SubtitleFetcher.Bootstrapping;
using SubtitleFetcher.Common;
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

        private static void HandleParseErrors(Options options)
        {
            if(options.ParseErrors.Any())
            {
                Environment.Exit(1);
            }
        }

        private static Options ParseOptions(string[] args)
        {
            var parser = new Parser(settings =>
                                        {
                                            settings.IgnoreUnknownArguments = false;
                                            settings.HelpWriter = Console.Error;
                                            settings.EnableDashDash = true;
                                        });
            var results = parser.ParseArguments<Options>(args);
            if(results.Tag == ParserResultType.Parsed)
                return ((Parsed<Options>)results).Value;

            var options = new Options();
            options.ParseErrors.AddRange(((NotParsed<Options>) results).Errors);
            return options;
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