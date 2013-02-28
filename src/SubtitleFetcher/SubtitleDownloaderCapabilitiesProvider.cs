using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleDownloader.Core;
using TvShowIdentification;

namespace SubtitleFetcher
{
    public class SubtitleDownloaderCapabilitiesProvider
    {
        private static readonly IEnumerable<string> EpisodeDownloaders = new[] { "Bierdopje", "OpenSubtitles", "Podnapisi", "S4U.se", "Sublight", "Subscene", "Swesub", "TvSubtitles" };
        private static readonly IEnumerable<string> MovieDownloaders = new[] { "MovieSubtitles", "OpenSubtitles", "S4U.se" };
        private static readonly IEnumerable<string> GeneralDownloaders = new[] { "MovieSubtitles", "OpenSubtitles", "Podnapisi", "S4U.se", "Sublight", "Subscene" };

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
            IEnumerable<string> downloaderNames = ReflectionHelper.GetAllConcreteImplementors<ISubtitleDownloader>().Select(downloader => downloader.Name.TrimSuffix("Downloader"));
            Console.WriteLine("The following subtitle downloaders are available:");
            foreach (string friendlyName in downloaderNames)
            {
                Console.WriteLine(friendlyName);
            }
        }
    }
}