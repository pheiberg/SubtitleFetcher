using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleDownloader.Core;

namespace SubtitleFetcher
{
	class Program
	{
		private enum HelpContext { Usage, Downloaders, Languages };

		static void Main(string[] args)
		{
			var downloaders = new List<ISubtitleDownloader>();
		    var options = GetOptions(args);
		    var logger = new Logger(options.Verbose);
            var tvdb = new TvdbSearcher();

            foreach (string downloader in options.DownloaderNames)
            {
                var dl = SubtitleDownloaderFactory.GetSubtitleDownloader(downloader);
                if (dl != null)
                {
                    downloaders.Add(dl);
                }
                else
                {
                    Console.WriteLine("\"{0}\" is not a known subtitle downloader.", downloader);
                }
            }

			InitIgnoredShows(options, logger);

			var serializer = new StateSerializer(options.StateFileName, logger, options.GiveupDays);
			var state = serializer.LoadState(options.Files);
		    
            var dict = state.Dict;
			var removeKeys = (from entry in dict.Values where ProcessFile(entry.File, options, downloaders, logger, tvdb) select entry.File).ToList();
		    foreach (string key in removeKeys)
		    {
		        dict.Remove(key);
		    }

			serializer.SaveState(state);
		}

	    private static Options GetOptions(IEnumerable<string> args)
	    {
	        var options = new Options {StateFileName = "DownloadState.xml", Language = "eng", Verbose = false};
	        foreach (string dl in SubtitleDownloaderFactory.GetSubtitleDownloaderNames())
	        {
	            options.DownloaderNames.Add(dl);
	        }

	        bool grabStateName = false;
	        bool grabIgnoreName = false;
	        bool grabGiveUpDays = false;
	        bool grabLanguage = false;
	        bool grabDownloaders = false;
	        bool checkHelpParameter = false;
	        foreach (string arg in args)
	        {
	            if (grabStateName)
	            {
	                options.StateFileName = arg;
	                grabStateName = false;
	            }
	            else if (grabIgnoreName)
	            {
	                options.IgnoreFileName = arg;
	                grabIgnoreName = false;
	            }
	            else if (grabLanguage)
	            {
	                options.Language = arg;
	                grabLanguage = false;
	            }
	            else if (grabDownloaders)
	            {
	                options.DownloaderNames.Clear();
	                foreach (var downloaderName in arg.Split(','))
	                {
	                    options.DownloaderNames.Add(downloaderName.Trim());
	                }
	                grabDownloaders = false;
	            }
	            else if (grabGiveUpDays)
	            {
	                int giveUpDays;
	                if (int.TryParse(arg, out giveUpDays))
	                {
	                    options.GiveupDays = giveUpDays;
	                }
	                else
	                {
	                    Console.WriteLine("Give up days is not an integer. Using default.");
	                }
	            }
	            else if (checkHelpParameter)
	            {
	                switch (arg)
	                {
	                    case "downloaders":
	                        ShowHelp(HelpContext.Downloaders);
	                        break;
	                    case "languages":
	                        ShowHelp(HelpContext.Languages);
	                        break;
	                }
	                checkHelpParameter = false;
	            }
	            else if (arg.StartsWith("-"))
	            {
	                if (arg == "-v" || arg == "--verbose")
	                    options.Verbose = true;
	                if (arg == "-s" || arg == "--state")
	                    grabStateName = true;
	                if (arg == "-i" || arg == "--ignore")
	                    grabIgnoreName = true;
	                if (arg == "-g" || arg == "--giveupdays")
	                    grabGiveUpDays = true;
	                if (arg == "-l" || arg == "--language")
	                    grabLanguage = true;
	                if (arg == "-d" || arg == "--downloaders")
	                    grabDownloaders = true;
	                if (arg == "-h" || arg == "--help")
	                    checkHelpParameter = true;
	            }
	            else
	            {
	                options.Files.Add(arg);
	            }
	        }
	        if (checkHelpParameter)
	        {
	            ShowHelp();
	        }
	        return options;
	    }

	    private static void InitIgnoredShows(Options options, Logger logger1)
	    {
	        if (options.IgnoreFileName == null) 
                return;

	        using (TextReader tr = new StreamReader(options.IgnoreFileName))
	        {
	            string s;
	            while ((s = tr.ReadLine()) != null)
	            {
	                options.IgnoreShows.Add(s.Trim());
	            }
	        }
	        logger1.Log("Ignore shows file loaded. {0} shows ignored.", options.IgnoreShows.Count);
	    }

	    static void ShowHelp(HelpContext context = HelpContext.Usage)
		{
			switch (context)
			{
				case HelpContext.Usage:
					Console.WriteLine("Usage: srtDownload.exe [options...] <directory>");
					Console.WriteLine("Options:");
					Console.WriteLine(" -v, --verbose     Shows more information, otherwise nothing is output (cron");
					Console.WriteLine("                   mode)");
					Console.WriteLine(" -s, --state       Path of state file (remembers when files were scanned)");
					Console.WriteLine(" -g, --giveupdays  The number of days after which the program gives up getting");
					Console.WriteLine("                   the subtitle and writes a .nosrt file.");
					Console.WriteLine(" -l, --language    The subtitle language requested (defaults to \"eng\").");
					Console.WriteLine(" -d, --downloaders Comma-separated list of downloaders to use.");
					Console.WriteLine(" -i, --ignore      Path of file containing ignored shows.");
					Console.WriteLine("                   A text file with a show name on each line. The name is the");
					Console.WriteLine("                   part of the the filename up to the season/episode id.");
					Console.WriteLine("                   E.g. \"Criminal.Minds.S08E07.HDTV.x264-LOL.mp4\" will be ");
					Console.WriteLine("                   ignored with a line of \"Criminal Minds\" in the file.");
					Console.WriteLine(" -h, --help        [|downloaders|languages] Show Help.");
					break;
				case HelpContext.Downloaders:
					Console.WriteLine("The following subtitle downloaders are available:");
					foreach (string dl in SubtitleDownloaderFactory.GetSubtitleDownloaderNames())
						Console.WriteLine(dl);
					break;
				case HelpContext.Languages:
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
					break;
			}
		}

	    private static readonly string[] AcceptedExtensions = new[] { ".avi", ".mkv", ".mp4" };

	    static bool ProcessFile(string fileName, Options options, IEnumerable<ISubtitleDownloader> subtitleDownloaders, Logger logger, TvdbSearcher tvdb)
		{
	        var nameParser = new EpisodeParser();
		    var ext = Path.GetExtension(fileName);
		    if (!AcceptedExtensions.Contains(ext))
				return true;

		    var path = Path.GetDirectoryName(fileName);
		    var file = Path.GetFileNameWithoutExtension(fileName);
		    var targetLocation = Path.Combine(path, file);
		    string targetSubtitleFile = targetLocation + ".srt";
			string targetNoSubtitleFile = targetLocation + ".nosrt";
			if (File.Exists(targetSubtitleFile) || File.Exists(targetNoSubtitleFile))
				return true;

	        var episodeIdentity = nameParser.ParseEpisodeInfo(Path.GetFileNameWithoutExtension(fileName));
	        if (string.IsNullOrEmpty(episodeIdentity.SeriesName))
		        return false;

		    if (options.IgnoreShows.Any(s => string.Equals(s, episodeIdentity.SeriesName)))
		    {
		        logger.Log("Ignoring {0}", fileName);
		        return true;
		    }

		    var seriesHits = tvdb.FindSeriesExact(episodeIdentity.SeriesName);
		    
		    logger.Log("Processing file {0}...", fileName);

		    var query = new EpisodeSearchQuery(episodeIdentity.SeriesName, episodeIdentity.Season, episodeIdentity.Episode, null) {LanguageCodes = new[] { options.Language }};

	        return subtitleDownloaders.Any(downloader => TryDownloadFile(episodeIdentity.SeriesName, episodeIdentity.Season, episodeIdentity.Episode, episodeIdentity.ReleaseGroup, targetSubtitleFile, query, downloader, nameParser, logger));
		}

	    private static bool TryDownloadFile(string name, int season, int episode, string releaseGroup, string targetSubtitleFile, EpisodeSearchQuery query, ISubtitleDownloader downloader, EpisodeParser nameParser, Logger logger)
	    {
	        try
	        {
	            var searchSubtitles = downloader.SearchSubtitles(query);
	            foreach (Subtitle subtitle in searchSubtitles)
	            {
	                var subtitleInfo = nameParser.ParseEpisodeInfo(subtitle.FileName);
	                var isMatch = string.Equals(name, subtitleInfo.SeriesName, StringComparison.InvariantCultureIgnoreCase) &&
	                              season == subtitleInfo.Season && episode == subtitleInfo.Episode &&
	                              string.Equals(releaseGroup, subtitleInfo.ReleaseGroup, StringComparison.InvariantCultureIgnoreCase);
	                if (!isMatch)
	                    continue;

	                logger.Log("Downloading subtitles from {0}...", downloader.GetName());
	                try
	                {
	                    List<FileInfo> subtitleFiles = downloader.SaveSubtitle(subtitle);
	                    FileInfo subtitleFile = subtitleFiles[0];
	                    logger.Log("Renaming from {0} to {1}...", subtitleFile.FullName, targetSubtitleFile);
	                    File.Delete(targetSubtitleFile);
	                    File.Move(subtitleFile.FullName, targetSubtitleFile);
	                    return true;
	                }
	                catch (Exception e)
	                {
	                    logger.Log("Downloader {0} failed: {1}", downloader.GetName(), e.Message);
	                }
	            }
	        }
	        catch (Exception e)
	        {
	            logger.Log("Downloader {0} failed: {1}", downloader.GetName(), e.Message);
	        }
	        return false;
	    }
	}
}