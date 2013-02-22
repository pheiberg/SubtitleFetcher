using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

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

            var capabilitiesProvider = new SubtitleDownloaderCapabilitiesProvider();

            if(options.ListDownloaders)
            {
                capabilitiesProvider.ListAvailableDownloaders();
                Environment.Exit(0);
            }

            if(options.ListLanguages)
            {
                capabilitiesProvider.ListAvailableLanguages();
                Environment.Exit(0);
            }

            var logger = new Logger(options.Debug);
            var downloaders = capabilitiesProvider.GetSubtitleDownloaders(options.DownloaderNames).ToList();
            var ignoredShows = GetIgnoredShows(logger, options.IgnoreFileName);

            var serializer = new StateSerializer(options.StateFileName, logger, options.GiveupDays, AcceptedExtensions);
            var state = serializer.LoadState(options.Files);
            var episodeParser = new EpisodeParser();
            var subtitleDownloadService = new SubtitleDownloadService(downloaders, options.Language, logger, episodeParser);
            var processor = new FileProcessor(episodeParser, logger, ignoredShows, subtitleDownloadService);
            var dict = state.Dict;
            var processedFiles = (from entry in dict.Values
                              where processor.ProcessFile(entry.File)
                              select entry.File).ToList();
            
            foreach (string key in processedFiles)
            {
                dict.Remove(key);
            }

            serializer.SaveState(state);
        }

	    private static IEnumerable<string> GetIgnoredShows(Logger logger, string ignoreFileName)
	    {
	        if (ignoreFileName == null) 
               yield break;

	        int count = 0;
	        using (TextReader reader = new StreamReader(ignoreFileName))
	        {
	            string line;
	            while ((line = reader.ReadLine()) != null)
	            {
	                yield return line.Trim();
	                count++;
	            }
	        }
	        logger.Log("Ignore shows file loaded. {0} shows ignored.", count);
	    }
	}
}