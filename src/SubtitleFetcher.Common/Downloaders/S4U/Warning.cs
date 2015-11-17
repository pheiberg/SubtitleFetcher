using System.Xml.Serialization;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class Warning
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
        [XmlText]
        public string Message { get; set; }
    }
}