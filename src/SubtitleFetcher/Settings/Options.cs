using System.Collections.Generic;
using CommandLine;
using SubtitleFetcher.Common.Logging;

namespace SubtitleFetcher.Settings
{
    public class Options
    {
        public Options()
        {
            DownloaderNames = new List<string>();
            ParseErrors = new List<Error>();
        }

        [Option('s', "state", Default = "FetcherState.xml", MetaValue = "FILE", HelpText = "Path of state FILE. The state file keeps track of previously unresolved scanned files.")]
        public string StateFileName { get; set; }

        [Option('i', "ignore", MetaValue = "FILE", HelpText = "Path of the FILE containing ignored shows. The file is a text file with a show name on each line. E.g. \"The.IT.Crowd.4x05.(PDTV-FoV).[VTV].mp4\" will be ignored with a line of \"the it crowd\" in the file.")]
        public string IgnoreFileName { get; set; }

        [Option('l', "languages", Separator = ',', HelpText = "The subtitle language requested as a list in the order of preference.", Default = new[] { "en" })]
        public IEnumerable<string> Languages { get; set; }

        [Option('g', "giveupdays", MetaValue = "INT", Default = 7, HelpText = "The number of days after which the program gives up getting a subtitle and writes a .nosrt file.")]
        public int GiveupDays { get; set; }

        [Option("logging", Default = LogLevel.Minimal, HelpText = "The level of logging used.")]
        public LogLevel Logging { get; set; }

        [Option("list-downloaders", HelpText = "Lists the available downloaders.", Default = false)]
        public bool ListDownloaders { get; set; }

        [Option("list-languages", HelpText = "Lists the available languages", Default = false)]
        public bool ListLanguages { get; set; }

        [Option('d', "downloaders", Separator = ',', HelpText = "(Default: All available) Download providers that should be used. Use list-downloaders, to list valid providers.")]
        public IList<string> DownloaderNames { get; set; }

        [Value(0, Default = ".", HelpText = "Path to the folder that contains the files that should be used in the subtitle search")]
        public string Directory { get; set; }
        
        public readonly IList<string> AcceptedExtensions = new List<string> { ".avi", ".mkv", ".mp4" };

        public IList<Error> ParseErrors { get; }
    }
}