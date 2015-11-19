using System.Collections.Generic;
using System.Xml.Serialization;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class Serie
    {
        [XmlElement("sub_hits")]
        public int SubHits { get; set; }
        [XmlElement("imdb")]
        public string Imdb { get; set; }
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("title_sv")]
        public string TitleSv { get; set; }
        [XmlElement("year")]
        public string Year { get; set; }
        [XmlElement("tvdb")]
        public string Tvdb { get; set; }
        [XmlElement("seasons")]
        public string Seasons { get; set; }
        [XmlElement("sub")]
        public List<Sub> Subs { get; set; }
    }
}