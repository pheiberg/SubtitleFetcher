using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using SubtitleFetcher.Common.Downloaders;
using SubtitleFetcher.Common.Downloaders.S4U;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Enhancement.Tvdb;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Logging;
using SubtitleFetcher.Common.Orchestration;
using SubtitleFetcher.Settings;

namespace SubtitleFetcher.Bootstrapping
{
    public class SubtitleDownloaderRegistry : Registry
    {
        public SubtitleDownloaderRegistry(Options options)
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory();
                scan.With(new RegisterAllSubtitleDownloadersConvention(options.DownloaderNames));
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
            For<EnhancerRegistry>().Singleton().Use<EnhancerRegistry>();
            For<OptionsParserSettings>().Use(new OptionsParserSettings
            {
                HelpWriter = Console.Error
            });
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
                bool isCompatibleType = typeof (ISubtitleDownloader).IsAssignableFrom(type)
                                     && !type.IsAbstract
                                     && !type.IsInterface;
                bool isNameMatchingDownloaderOptions = !_downloaderNames.Any() 
                                                    || _downloaderNames.Any(d => type.Name.StartsWith(d, StringComparison.OrdinalIgnoreCase));
                 
                if (!isCompatibleType || !isNameMatchingDownloaderOptions)
                    return;

                registry.For(typeof(ISubtitleDownloader)).Add(type).Named(type.Name);
                registry.For<IEpisodeSubtitleDownloader>().Add<SubtitleDownloaderWrapper>()
                    .Ctor<ISubtitleDownloader>().IsNamedInstance(type.Name);
            }
        }
    }
}