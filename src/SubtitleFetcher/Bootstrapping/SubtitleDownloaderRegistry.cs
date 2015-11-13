using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Enhancement.Tvdb;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Orchestration;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher.Bootstrapping
{
    public class SubtitleDownloaderRegistry : Registry
    {
        public SubtitleDownloaderRegistry(Options options)
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory();
                if (options.DownloaderNames.Any())
                {
                    scan.With(new RegisterAllSubtitleDownloadersConvention(options.DownloaderNames));
                }
                else
                {
                    scan.AddAllTypesOf<ISubtitleDownloader>();
                }
                scan.AddAllTypesOf<IEnhancer>();
            });
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<ITvdbSearcher>();
                scan.WithDefaultConventions();
            });

            For<ILogger>().Use<Logger>().Ctor<LogLevel>().Is(options.Logging);
            For<IStateSerializer>().Use<StateSerializer>().Ctor<string>("stateFileName").Is(options.StateFileName);
            var languages = options.Languages.Select(KnownLanguages.GetLanguageByTwoLetterIso);
            For<LanguageSettings>().Use(new LanguageSettings(languages));
            For<FileTypeSettings>().Use(new FileTypeSettings(options.AcceptedExtensions));
            For<ISubtitleDownloadService>().Use(CreateSubtitleDownloadService);
            For<EnhancerRegistry>().Singleton().Use<EnhancerRegistry>();
        }

        private static SubtitleDownloadService CreateSubtitleDownloadService(IContext context)
        {
            Func<ISubtitleDownloader, SubtitleDownloaderWrapper> createSubtitleDownloader = sd => new SubtitleDownloaderWrapper(sd, context.GetInstance<IEpisodeParser>(), context.GetInstance<ILogger>(), context.GetInstance<IFileSystem>());
            IEnumerable<SubtitleDownloaderWrapper> episodeSubtitleDownloaders = context.GetAllInstances<ISubtitleDownloader>().Select(createSubtitleDownloader);
            return new SubtitleDownloadService(episodeSubtitleDownloaders, context.GetInstance<IEnhancementProvider>(), context.GetInstance<SubtitleRanker>());
        }

        private class RegisterAllSubtitleDownloadersConvention : IRegistrationConvention
        {
            private readonly IEnumerable<string> _downloaderNames;

            public RegisterAllSubtitleDownloadersConvention(IEnumerable<string> downloaderNames)
            {
                _downloaderNames = downloaderNames;
            }

            public void Process(Type type, Registry registry)
            {
                if (typeof(ISubtitleDownloader).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface &&
                    _downloaderNames.Any(d => type.Name.StartsWith(d, StringComparison.OrdinalIgnoreCase)))
                {
                    registry.For(typeof(ISubtitleDownloader)).Add(type);
                }
            }
        }
    }
}