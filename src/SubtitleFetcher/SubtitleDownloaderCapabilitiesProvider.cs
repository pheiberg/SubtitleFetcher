using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleDownloader.Core;

namespace SubtitleFetcher
{
    public class SubtitleDownloaderCapabilitiesProvider
    {
        private static readonly IEnumerable<string> EpisodeDownloaders = new[] { "Bierdopje", "OpenSubtitles", "Podnapisi", "S4U.se", "Sublight", "Subscene", "TvSubtitles" };
        private static readonly IEnumerable<string> MovieDownloaders = new[] { "MovieSubtitles", "OpenSubtitles", "S4U.se" };
        private static readonly IEnumerable<string> GeneralDownloaders = new[] { "MovieSubtitles", "OpenSubtitles", "Podnapisi", "S4U.se", "Sublight", "Subscene" };

        public IEnumerable<ISubtitleDownloader> GetSubtitleDownloaders(IEnumerable<string> downloaderNames, SubtitleType type = SubtitleType.TvShow)
        {
            var compatibleDowloaders = GetDownloadersForType(type).ToList();
            var chosenDownloaders = downloaderNames.Any() ? downloaderNames : SubtitleDownloaderFactory.GetSubtitleDownloaderNames();
            
            foreach (string downloaderName in chosenDownloaders)
            {
                var downloader = SubtitleDownloaderFactory.GetSubtitleDownloader(downloaderName);

                if (downloader != null)
                {
                    if(compatibleDowloaders.Contains(downloader.GetName()))
                        yield return downloader;
                }
                else
                {
                    Console.WriteLine("\"{0}\" is not a known subtitle downloader.", downloaderName);
                }
            }
        }

        private IEnumerable<string> GetDownloadersForType(SubtitleType type)
        {
            switch(type)
            {
                case SubtitleType.TvShow:
                    return EpisodeDownloaders;
                case SubtitleType.Movie:
                    return MovieDownloaders;
                default:
                    return GeneralDownloaders;
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
        TvShow,
        General
    }
}