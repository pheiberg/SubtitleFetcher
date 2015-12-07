using System.Diagnostics;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common
{
    [DebuggerDisplay("{SeriesName}.S{Season}.E{Episode}-{ReleaseGroup}-{Language.Name}")]
    public class Subtitle
    {
        public Subtitle(string id, string fileName, Language language)
        {
            Id = id;
            FileName = fileName;
            Language = language;
        }
        
        public string FileName { get; set; }
        public Language Language { get; set; }
        public string Id { get; set; }
        public string SeriesName { get; set; }
        public int? Season { get; set; }
        public int? Episode { get; set; }
        public int? EndEpisode { get; set; }
        public string ReleaseGroup { get; set; }
    }
}