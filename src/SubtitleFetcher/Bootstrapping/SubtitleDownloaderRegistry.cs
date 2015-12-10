using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using SubtitleFetcher.Common.Downloaders;
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

            For<IFileSystem>().Use<FileSystem>();
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
            
            public void ScanTypes(TypeSet types, Registry registry)
            {
                var matchingTypes = types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)
                                         .Where(type => typeof(ISubtitleDownloader).IsAssignableFrom(type)
                                         && IsNameMatchingDownloaderOptions(type.Name));

                matchingTypes.Each(type =>
                {
                    registry.For(typeof(ISubtitleDownloader)).Add(type).Named(type.Name);
                    registry.For<IEpisodeSubtitleDownloader>().Add<SubtitleDownloaderWrapper>()
                            .Ctor<ISubtitleDownloader>().IsNamedInstance(type.Name);
                });
            }

            private bool IsNameMatchingDownloaderOptions(string name)
            {
                return !_downloaderNames.Any()
                       || _downloaderNames.Any(d => name.StartsWith(d, StringComparison.OrdinalIgnoreCase));
            }
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach(var item in items)
            {
                action(item);
            }
            return items;
        }
    }
}