using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Logging;

namespace SubtitleFetcher
{
    public class Options
    {
        public Options()
        {
            DownloaderNames = new List<string>();
            Files = new List<string>();
        }

        [Option('s', "state", DefaultValue = "FetcherState.xml", MetaValue = "FILE", HelpText = "Path of state FILE. The state file keeps track of previously unresolved scanned files.")]
        public string StateFileName { get; set; }

        [Option('i', "ignore", MetaValue = "FILE", HelpText = "Path of the FILE containing ignored shows. The file is a text file with a show name on each line. E.g. \"The.IT.Crowd.4x05.(PDTV-FoV).[VTV].mp4\" will be ignored with a line of \"the it crowd\" in the file.")]
        public string IgnoreFileName { get; set; }

        [OptionArray('l', "languages", DefaultValue = new []{"eng"}, HelpText = "The subtitle language requested as a list in the order of preference.")]
        public string[] Languages { get; set; }

        [Option('g', "giveupdays", MetaValue = "INT", DefaultValue = 7, HelpText = "The number of days after which the program gives up getting a subtitle and writes a .nosrt file.")]
        public int GiveupDays { get; set; }

        [Option("logging", DefaultValue = LogLevel.Minimal, HelpText = "The level of logging used.")]
        public LogLevel Logging { get; set; }

        [Option("list-downloaders", HelpText = "Lists the available downloaders.", DefaultValue = false)]
        public bool ListDownloaders { get; set; }

        [Option("list-languages", HelpText = "Lists the available languages", DefaultValue = false)]
        public bool ListLanguages { get; set; }

        [OptionList('d', "downloaders", ',')]
        public IList<string> DownloaderNames { get; set; }

        [ValueList(typeof(List<string>))]
        public IList<string> Files { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public readonly IList<string> AcceptedExtensions = new List<string> { ".avi", ".mkv", ".mp4" };
    }
}