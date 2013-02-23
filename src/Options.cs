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
        }

        [Option('s', "state", DefaultValue = "FetcherState.xml", MetaValue = "FILE", HelpText = "Path of state FILE (remembers when files were scanned)")]
        public string StateFileName { get; set; }

        [Option('i', "ignore", MetaValue = "FILE", HelpText = "Path of the FILE containing ignored shows. A text file with a show name on each line. The name is the part of the the filename up to the season/episode id. E.g. \"Criminal.Minds.S08E07.HDTV.x264-LOL.mp4\" will be ignored with a line of \"Criminal Minds\" in the file.")]
        public string IgnoreFileName { get; set; }

        [OptionArray('l', "languages", DefaultValue = new []{"eng"}, HelpText = "The subtitle language requested as a list in the order of preference.")]
        public string[] Languages { get; set; }

        [Option('g', "giveupdays", MetaValue = "INT", DefaultValue = 7, HelpText = "The number of days after which the program gives up getting a subtitle and writes a .nosrt file")]
        public int GiveupDays { get; set; }

        [Option("debug", DefaultValue = false, HelpText = "If logging should be turned on")]
        public bool Debug { get; set; }

        [Option("list-downloaders", HelpText = "Lists the available downloaders", DefaultValue = false)]
        public bool ListDownloaders { get; set; }

        [Option("list-languages", HelpText = "Lists the available languages", DefaultValue = false, MutuallyExclusiveSet = "")]
        public bool ListLanguages { get; set; }

        [OptionList('d', "downloaders", Separator = ' ')]
        public IList<string> DownloaderNames { get; private set; }

        [ValueList(typeof(List<string>))]
        public IList<string> Files { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}