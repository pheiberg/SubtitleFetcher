using System.Collections.Generic;
using System.Xml.Serialization;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class Info
    {
        public string status { get; set; }
        public string version { get; set; }
        public int dl_left { get; set; }
        public string query { get; set; }
        public int hits { get; set; }
        public int hits_movie { get; set; }
        public int hits_movie_sub { get; set; }
        public int hits_serie { get; set; }
        public int hits_serie_sub { get; set; }
        [XmlElement("error")]
        public List<Error> Errors { get; set; }
        [XmlElement("warning")]
        public List<Warning> Warnings { get; set; }
    }
}