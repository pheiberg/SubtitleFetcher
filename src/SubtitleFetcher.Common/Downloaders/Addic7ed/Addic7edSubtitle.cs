using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class Addic7edSubtitle
    {
        public int Season { get; set; }

        public int Episode { get; set; }

        public string DowloadLink { get; set; }

        public string Language { get; set; }

        public string Version { get; set; }

        public bool Corrected { get; set; }

        public bool HearingImpaired { get; set; }

        public bool HighDefinition { get; set; }
    }
}