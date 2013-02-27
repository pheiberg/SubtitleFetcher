using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using TvShowIdentification;

namespace SubtitleFetcher
{
	class Program
	{
        private static readonly string[] AcceptedExtensions = new[] { ".avi", ".mkv", ".mp4" };
	    private static readonly SubtitleDownloaderCapabilitiesProvider CapabilitiesProvider = new SubtitleDownloaderCapabilitiesProvider();

	    static void Main(string[] args)
        {
            var options = ParseOptions(args);
	        HandleHelpRequests(options);
            ProcessFiles(options);
        }

	    private static Options ParseOptions(string[] args)
	    {
	        var options = new Options();
	        var parser = new Parser(new ParserSettings(Console.Error));
	        if (!parser.ParseArgumentsStrict(args, options))
	        {
	            Environment.Exit(1);
	        }
	        return options;
	    }

	    private static void HandleHelpRequests(Options options)
	    {
	        if (options.ListDownloaders)
	        {
	            CapabilitiesProvider.ListAvailableDownloaders();
	            Environment.Exit(0);
	        }

	        if (options.ListLanguages)
	        {
	            CapabilitiesProvider.ListAvailableLanguages();
	            Environment.Exit(0);
	        }
	    }

	    private static void ProcessFiles(Options options)
	    {
	        var logger = new Logger(options.Logging);
	        var fileSystem = new FileSystem(AcceptedExtensions, logger);
            var processor = CreateFileProcessor(options, logger, fileSystem);

	        var serializer = new StateSerializer(options.StateFileName, logger);
	        var state = serializer.LoadState();
	        state.Cleanup(options.GiveupDays, fileSystem.CreateNosrtFile);

	        var filesToProcess = fileSystem.GetFilesToProcess(options.Files);
	        var failedFiles = (filesToProcess.Where(file => !processor.ProcessFile(file))).ToList();

	        state.Update(failedFiles);
	        serializer.SaveState(state);
	    }

	    private static FileProcessor CreateFileProcessor(Options options, Logger logger, FileSystem fileSystem)
	    {
	        var episodeParser = new EpisodeParser();
	        var subtitleDownloadProvider = new SubtitleDownloadProvider(new SwesubDownloader.SwesubDownloader(logger, new TvdbSearcher()), episodeParser, logger, fileSystem);
            var downloaders = new List<ISubtitleDownloadProvider> { subtitleDownloadProvider };
	        //CapabilitiesProvider.GetSubtitleDownloaders(options.DownloaderNames);
                //.Select(downloader => new SubtitleDownloadProvider(downloader, episodeParser, logger, fileSystem));
	        var ignoredShows = fileSystem.GetIgnoredShows(options.IgnoreFileName);
	        var subtitleDownloadService = new SubtitleDownloadService(downloaders, options.Languages);
	        var processor = new FileProcessor(episodeParser, logger, ignoredShows, subtitleDownloadService);
	        return processor;
	    }
	}
}