using System;
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
            var fileSystem = new FileSystem(AcceptedExtensions, logger);
            
            var processor = CreateFileProcessor(options, logger, fileSystem, capabilitiesProvider);

            var serializer = new StateSerializer(options.StateFileName, logger);
            var state = serializer.LoadState();
            state.Cleanup(options.GiveupDays, fileSystem.CreateNosrtFile);

            var filesToProcess = fileSystem.GetFilesToProcess(options.Files); 
            var failedFiles = (filesToProcess.Where(file => !processor.ProcessFile(file))).ToList();

            state.Update(failedFiles);
            serializer.SaveState(state);
        }

	    private static FileProcessor CreateFileProcessor(Options options, Logger logger, FileSystem fileSystem, SubtitleDownloaderCapabilitiesProvider capabilitiesProvider)
	    {
	        var episodeParser = new EpisodeParser();
	        var downloaders = capabilitiesProvider.GetSubtitleDownloaders(options.DownloaderNames)
                .Select(downloader => new SubtitleDownloadProvider(downloader, episodeParser, logger, fileSystem));
	        var ignoredShows = fileSystem.GetIgnoredShows(options.IgnoreFileName);
	        var subtitleDownloadService = new SubtitleDownloadService(downloaders, options.Languages);
	        var processor = new FileProcessor(episodeParser, logger, ignoredShows, subtitleDownloadService);
	        return processor;
	    }
	}
}