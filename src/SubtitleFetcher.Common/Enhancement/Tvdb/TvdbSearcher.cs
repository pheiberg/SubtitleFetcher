using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace SubtitleFetcher.Common.Enhancement.Tvdb
{
    public class TvdbSearcher : ITvdbSearcher
    {
        private const string ServiceLocation = "http://thetvdb.com/api/GetSeries.php?seriesname={0}";
        private readonly IDictionary<string, IEnumerable<TvdbSeries>> _cache = new Dictionary<string, IEnumerable<TvdbSeries>>(); 

        public IEnumerable<TvdbSeries> FindSeries(string name)
        {
            if (_cache.ContainsKey(name))
                return _cache[name];

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
            _cache[name] = results;
            return results;
        }

        public TvdbSeries FindSeriesExact(string name)
        {
            var seriesHits = FindSeries(name);
            var exactHit = seriesHits.SingleOrDefault(s => string.Equals(s.SeriesName, name, StringComparison.OrdinalIgnoreCase)) ?? new TvdbSeries();
            return exactHit;
        }
    }
}