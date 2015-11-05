using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Download;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Parsing;

namespace SubtitleFetcher
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
            });
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType<ITvdbSearcher>();
                scan.WithDefaultConventions();
            });

            For<ILogger>().Use<Logger>().Ctor<LogLevel>().Is(options.Logging);
            For<IStateSerializer>().Use<StateSerializer>().Ctor<string>("stateFileName").Is(options.StateFileName);
            For<LanguageSettings>().Use(new LanguageSettings(options.Languages));
            For<FileTypeSettings>().Use(new FileTypeSettings(options.AcceptedExtensions));
            For<ISubtitleDownloadService>().Use(CreateSubtitleDownloadService);
        }

        private static SubtitleDownloadService CreateSubtitleDownloadService(IContext context)
        {
            Func<ISubtitleDownloader, EpisodeSubtitleDownloader> createSubtitleDownloader = sd => new EpisodeSubtitleDownloader(sd, context.GetInstance<IEpisodeParser>(), context.GetInstance<ILogger>(), context.GetInstance<IFileSystem>());
            IEnumerable<EpisodeSubtitleDownloader> episodeSubtitleDownloaders = context.GetAllInstances<ISubtitleDownloader>().Select(createSubtitleDownloader);
            return new SubtitleDownloadService(episodeSubtitleDownloaders);
        }

        private class RegisterAllSubtitleDownloadersConvention : IRegistrationConvention
        {
            private readonly IEnumerable<string> downloaderNames;

            public RegisterAllSubtitleDownloadersConvention(IEnumerable<string> downloaderNames)
            {
                this.downloaderNames = downloaderNames;
            }

            public void Process(Type type, Registry registry)
            {
                if (typeof(ISubtitleDownloader).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface &&
                    downloaderNames.Any(d => type.Name.StartsWith(d, StringComparison.OrdinalIgnoreCase)))
                {
                    registry.For(typeof(ISubtitleDownloader)).Add(type);
                }
            }
        }
    }
}