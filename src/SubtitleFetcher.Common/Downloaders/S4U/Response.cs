using System.Collections.Generic;
using System.Xml.Serialization;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    [XmlRoot("xmlresult")]
    public class Response
    {
        [XmlElement("info")]
        public Info Info { get; set; }

        [XmlElement("serie")]
        public List<Serie> Series { get; set; }
    }
}