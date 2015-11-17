using System.Xml;
using System.Xml.Serialization;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class Sub
    {
        [XmlElement("id")]
        public int Id { get; set; }
        [XmlElement("rls")]
        public string Release { get; set; }
        [XmlElement("rls_group")]
        public string ReleaseGroup { get; set; }
        [XmlElement("translate")]
        public string Translate { get; set; }
        [XmlElement("file_name")]
        public string FileName { get; set; }
        [XmlElement("file_type")]
        public string FileType { get; set; }
        [XmlElement("file_count")]
        public int FileCount { get; set; }
        [XmlElement("file_relation")]
        public string FileRelation { get; set; }
        [XmlElement("download_file")]
        public string DownloadFile { get; set; }
        [XmlElement("download_zip")]
        public string DownloadZip { get; set; }
        [XmlElement("tvdb_ep")]
        public string TvdbEpisode { get; set; }
        [XmlElement("ep_title")]
        public string EpisodeTitle { get; set; }
        [XmlElement("season")]
        public int Season { get; set; }
        [XmlElement("episode")]
        public int Episode { get; set; }
    }
}