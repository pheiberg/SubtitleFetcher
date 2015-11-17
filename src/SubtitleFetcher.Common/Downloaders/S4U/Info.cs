using System.Collections.Generic;
using System.Xml.Serialization;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class Info
    {
        [XmlElement("status")]
        public bool Status { get; set; }
        [XmlElement("version")]
        public string Version { get; set; }
        [XmlElement("dl_left")]
        public int DownloadsLeft { get; set; }
        [XmlElement("query")]
        public string Query { get; set; }
        [XmlElement("hits")]
        public int Hits { get; set; }
        [XmlElement("hits_movie")]
        public int HitsMovie { get; set; }
        [XmlElement("hits_movie_sub")]
        public int HitsMovieSub { get; set; }
        [XmlElement("hits_serie")]
        public int HitsSerie { get; set; }
        [XmlElement("hits_serie_sub")]
        public int HitsSerieSub { get; set; }
        [XmlElement("error")]
        public List<Error> Errors { get; set; }
        [XmlElement("warning")]
        public List<Warning> Warnings { get; set; }
    }
}