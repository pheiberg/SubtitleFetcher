using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace TvShowIdentification
{
    public class TvdbSearcher
    {
        private const string ApiKey = "E1644B56FF8A498A";
        private const string ServiceLocation = "http://thetvdb.com/api/GetSeries.php?seriesname={0}";
        private readonly IDictionary<string, IEnumerable<TvdbSeries>> cache = new Dictionary<string, IEnumerable<TvdbSeries>>(); 

        public IEnumerable<TvdbSeries> FindSeries(string name)
        {
            if (cache.ContainsKey(name))
                return cache[name];

            var client = new WebClient();
            var resultXml = client.DownloadString(string.Format(ServiceLocation, Uri.EscapeDataString(name)));
            XDocument document = XDocument.Parse(resultXml);
            var results = (from s in document.Descendants("Series")
                select new TvdbSeries
                       {
                           Banner = (string)s.Element("banner"),
                           FirstAired = (string)s.Element("FirstAired"),
                           ImdbId = (string)s.Element("IMDB_ID"),
                           Language = (string)s.Element("language"),
                           Overview = (string)s.Element("Overview"),
                           SeriesId = (int?)s.Element("seriesid"),
                           SeriesName = (string)s.Element("SeriesName")
                       }).ToList();
            cache[name] = results;
            return results;
        }

        public TvdbSeries FindSeriesExact(string name)
        {
            var seriesHits = FindSeries(name);
            var exactHit = seriesHits.SingleOrDefault(s => string.Equals(s.SeriesName, name, StringComparison.InvariantCultureIgnoreCase)) ?? new TvdbSeries();
            return exactHit;
        }
    }
}