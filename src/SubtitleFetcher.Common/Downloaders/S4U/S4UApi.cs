using System;
using System.IO;
using System.Xml.Serialization;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class S4UApi
    {
        private readonly string _baseUrl;
        private readonly LimitsBuilder _limitsBuilder;
        private readonly XmlSerializer _xmlSerializer;
        private const string SiteUrl = "http://api.s4u.se/1.0";

        public S4UApi(S4USettings settings)
        {
            _baseUrl =  $"{SiteUrl}/{settings.ApiKey}/xml/serie/";
            _limitsBuilder = new LimitsBuilder();
            _xmlSerializer = new XmlSerializer(typeof(Response));
        }

        public Response SearchByImdbId(int imdbId, S4ULimits limits = null)
        {
            return Search("imdb", imdbId.ToString("0000000"), limits);
        }

        public Response SearchByTvdbId(int tvdbId, int? tvdbEpisodeId = null, S4ULimits limits = null)
        {
            return Search("tvdb", tvdbId.ToString(), limits);
        }

        public Response SearchByTitle(string title, S4ULimits limits = null)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            
            return Search("title", title, limits);
        }

        public Response SearchByRelease(string release, S4ULimits limits = null)
        {
            if (release == null) throw new ArgumentNullException(nameof(release));

            return Search("rls", release, limits);
        }

        private Response Search(string searchType, string release, S4ULimits limits)
        {
            string limitsString = _limitsBuilder.BuildString(limits);
            var uri = new Uri(_baseUrl + searchType + "/" + release + limitsString);
            using (var xmlStream = new WebDownloader().OpenRead(uri))
            {
                return (Response) _xmlSerializer.Deserialize(xmlStream);
            }
        }
    }
}
