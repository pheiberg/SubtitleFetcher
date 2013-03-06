using System;
using CommandLine;
using StructureMap;

namespace SubtitleFetcher
{
    class Program
    {
        private static readonly SubtitleDownloaderCapabilitiesProvider CapabilitiesProvider = new SubtitleDownloaderCapabilitiesProvider();

        static void Main(string[] args)
        {
            var options = ParseOptions(args);
            HandleHelpRequests(options);

            var application = InitializeApplication(options);
            application.Run(options);
        }

        private static Options ParseOptions(string[] args)
        {
            var parser = new Parser(settings =>
                                        {
                                            settings.IgnoreUnknownArguments = false;
                                            settings.HelpWriter = Console.Error;
                                            settings.MutuallyExclusive = true;
                                        });
            var options = parser.ParseArguments<Options>(args, () => Environment.Exit(1));
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