using System.Collections.Generic;
using System.Xml.Serialization;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class Serie
    {
        public int sub_hits { get; set; }
        public string imdb { get; set; }
        public string title { get; set; }
        public string title_sv { get; set; }
        public string year { get; set; }
        public string tvdb { get; set; }
        public string seasons { get; set; }
        [XmlElement("sub")]
        public List<Sub> Subs { get; set; }
    }
}