using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Bootstrapping;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher
{
    public class SubtitleDownloaderCapabilitiesProvider
    {
        public void ListAvailableLanguages()
        {
            Console.WriteLine("The following languages are available:");
            int i = 0;
            foreach (var language in KnownLanguages.AllLanguages.OrderBy(l => l.Name))
            {
                Console.Write($"{language.TwoLetterIsoName} - {language.Name}\t");
                if (++i % 3 == 0)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }

        public void ListAvailableDownloaders()
        {
            IEnumerable<string> downloaderNames = ReflectionHelper.GetAllConcreteImplementors<ISubtitleDownloader>().Select(downloader => downloader.Name.TrimSuffix("Downloader"));
            Console.WriteLine("The following subtitle downloaders are available:");
            foreach (string friendlyName in downloaderNames)
            {
                Console.WriteLine(friendlyName);
            }
        }
    }
}