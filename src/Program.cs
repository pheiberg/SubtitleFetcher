using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using SubtitleDownloader.Core;

namespace SubtitleFetcher
{
	class Program
	{
        private static readonly string[] AcceptedExtensions = new[] { ".avi", ".mkv", ".mp4" };

        static void Main(string[] args)
        {
            var options = new Options();
            var parser = new Parser(new ParserSettings(Console.Error));
            if (!parser.ParseArgumentsStrict(args, options))
            {
                Environment.Exit(1);
            }

            if(options.ListDownloaders)
            {
                ListAvailableDownloaders();
                Environment.Exit(0);
            }

            if(options.ListLanguages)
            {
                ListAvailableLanguages();
                Environment.Exit(0);
            }

            var logger = new Logger(options.Debug);
            var tvdb = new TvdbSearcher();
            var downloaders = GetSubtitleDownloaders(options).ToList();
            var ignoredShows = GetIgnoredShows(options, logger);

            var serializer = new StateSerializer(options.StateFileName, logger, options.GiveupDays, AcceptedExtensions);
            var state = serializer.LoadState(options.Files);

            var processor = new FileProcessor(new EpisodeParser(), downloaders, logger, ignoredShows, options);
            var dict = state.Dict;
            var removeKeys = (from entry in dict.Values
                              where processor.ProcessFile(entry.File)
                              select entry.File).ToList();
            foreach (string key in removeKeys)
            {
                dict.Remove(key);
            }

            serializer.SaveState(state);
        }

	    private static IEnumerable<ISubtitleDownloader> GetSubtitleDownloaders(Options options)
	    {
	        foreach (string downloaderName in options.DownloaderNames)
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

	    private static void ListAvailableLanguages()
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

	    private static void ListAvailableDownloaders()
	    {
	        Console.WriteLine("The following subtitle downloaders are available:");
			foreach (string downloaderName in SubtitleDownloaderFactory.GetSubtitleDownloaderNames())
			{
			    Console.WriteLine(downloaderName);
			}
	    }

	    private static IEnumerable<string> GetIgnoredShows(Options options, Logger logger)
	    {
	        if (options.IgnoreFileName == null) 
               yield break;

	        int count = 0;
	        using (TextReader tr = new StreamReader(options.IgnoreFileName))
	        {
	            string s;
	            while ((s = tr.ReadLine()) != null)
	            {
	                yield return s.Trim();
	                count++;
	            }
	        }
	        logger.Log("Ignore shows file loaded. {0} shows ignored.", count);
	    }
	}
}