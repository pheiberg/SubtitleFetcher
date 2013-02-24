using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleDownloader.Core;

namespace SubtitleFetcher
{
    public class SubtitleDownloaderCapabilitiesProvider
    {
        public IEnumerable<ISubtitleDownloader> GetSubtitleDownloaders(IEnumerable<string> downloaderNames, SubtitleType type = SubtitleType.TvShow)
        {
            var dowloadersToUse = downloaderNames.Any() ? downloaderNames : GetDownloadersForType(type);
            foreach (string downloaderName in dowloadersToUse)
            {
                var downloader = SubtitleDownloaderFactory.GetSubtitleDownloader(downloaderName);
                if (downloader != null)
                {
                    yield return downloader;
                }
                else
                {
                    Console.WriteLine("\"{0}\" is not a known subtitle downloader.", downloaderName);
                }
            }
        }

        private static IEnumerable<string> GetDownloadersForType(SubtitleType type)
        {
            var subtitleDownloaderNames = SubtitleDownloaderFactory.GetSubtitleDownloaderNames();
            switch(type)
            {
                case SubtitleType.TvShow:
                    return subtitleDownloaderNames.Where(s => s != "MovieSubtitles");
                default:
                    return subtitleDownloaderNames.Where(s => s != "TvSubtitles");
            }
            
        }

        public void ListAvailableLanguages()
        {
            Console.WriteLine("The following languages are possible:");
            Console.WriteLine("alb Albanian    fre French      nor Norwegian");
            Console.WriteLine("ara Arabic      ger German      per Persian");
            Console.WriteLine("bel Belarusian  gre Greek       pol Polish");
            Console.WriteLine("bos Bosnian     heb Hebrew      por Portuguese");
            Console.WriteLine("bul Bulgarian   hin Hindi       rum Romanian");
            Console.WriteLine("cat Catalan     hun Hungarian   rus Russian");
            Console.WriteLine("chi Chinese     ice Icelandic   srp Serbian");
            Console.WriteLine("hrv Croatian    ind Indonesian  slo Slovak");
            Console.WriteLine("cze Czech       gle Irish       slv Slovenian");
            Console.WriteLine("dan Danish      ita Italian     spa Spanish");
            Console.WriteLine("nld Dutch       jpn Japanese    swe Swedish");
            Console.WriteLine("dut Dutch       kor Korean      tha Thai");
            Console.WriteLine("eng English     lav Latvian     tur Turkish");
            Console.WriteLine("est Estonian    lit Lithuanian  ukr Ukrainian");
            Console.WriteLine("fin Finnish     mac Macedonian  vie Vietnamese");

        }

        public void ListAvailableDownloaders()
        {
            Console.WriteLine("The following subtitle downloaders are available:");
            foreach (string downloaderName in SubtitleDownloaderFactory.GetSubtitleDownloaderNames())
            {
                Console.WriteLine(downloaderName);
            }
        }
    }

    public enum SubtitleType
    {
        Movie,
        TvShow
    }
}