using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace SubtitleFetcher
{
    public class Options
    {
        public Options()
        {
            DownloaderNames = new List<string>();
            Files = new List<string>();
            IgnoreShows = new List<string>();
        }

        [Option('s', "state", DefaultValue = "srtDownload.xml", MetaValue = "FILE", HelpText = "Path of state FILE (remembers when files were scanned)")]
        public string StateFileName { get; set; }

        [Option('i', "ignore", MetaValue = "FILE", HelpText = "Path of the FILE containing ignored shows.")]
        public string IgnoreFileName { get; set; }

        [Option('l', "Language", DefaultValue = "eng", HelpText = "The subtitle language requested (defaults to \"eng\").")]
        public string Language { get; set; }

        [Option('g', "giveupdays", MetaValue = "INT", DefaultValue = 7, HelpText = "The number of days after which the program gives up getting a subtitle and writes a .nosrt file")]
        public int GiveupDays { get; set; }

        [Option('v', "verbose", DefaultValue = false, HelpText = "If logging should be verbose")]
        public bool Verbose { get; set; }

        [OptionList('d', "downloaders", Separator = ',')]
        public IList<string> DownloaderNames { get; private set; }

        [ValueList(typeof(List<string>))]
        public IList<string> Files { get; set; }
        
        public IList<string> IgnoreShows { get; private set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}