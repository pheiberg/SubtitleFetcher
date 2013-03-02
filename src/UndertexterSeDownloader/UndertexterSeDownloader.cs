using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SubtitleDownloader.Core;
using SubtitleFetcher.Common;

namespace UndertexterSeDownloader
{
    public class UndertexterSeDownloader : DownloaderBase, ISubtitleDownloader, IDownloadCapabilitiesProvider
    {
        private const string GetShowsPage = "http://undertexter.se";
        private const string ShowPageUrl = "http://www.undertexter.se/?p=serier&id={0}";
        private const string DownloadUrlFormat = "http://www.undertexter.se/laddatext.php?id={0}";
        private static readonly Regex ShowListExpression = new Regex("<option value=\\\"http://www.undertexter.se/\\?p=serier&id=(?<id>\\d+)\\\"\\s?>(?<name>[\\w\\W]*?)</option>");
        private const string ExtractSubtitleExpressionFormat = "<a.*title=\"(?<release>.*)\"\\salt=\".*\"\\shref=\"laddatext.php\\?id=(?<id>\\d+)\">";
        private const string DownloaderName = "undertexter.se";

        public UndertexterSeDownloader(ILogger logger, IEpisodeParser parser) 
            : base(DownloaderName, logger, parser, ShowPageUrl, ExtractSubtitleExpressionFormat, DownloadUrlFormat)
        {

        }

        protected override string GetShowId(string name)
        {
            var shows = GetAllSeries();
            var matching = shows.SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            return matching == null ? null : matching.Id.ToString(CultureInfo.InvariantCulture);
        }

        private IEnumerable<Series> GetAllSeries()
        {
            var client = GetWebClient();
            var page = client.DownloadString(GetShowsPage);
            var matches = ShowListExpression.Matches(page);
            return from Match match in matches 
                   let id = int.Parse(match.Groups["id"].Value.Trim()) 
                   let name = match.Groups["name"].Value.Trim(' ', '\n').Replace("&", "and").Replace(":", "") 
                   select new Series(id, name);
        }

        public override bool CanHandleEpisodeSearchQuery
        {
            get { return true; }
        }

        public override bool CanHandleImdbSearchQuery
        {
            get { return false; }
        }

        public override bool CanHandleSearchQuery
        {
            get { return false; }
        }

        private class Series
        {
            private readonly int id;
            private readonly string name;

            public Series(int id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public string Name
            {
                get { return name; }
            }

            public int Id
            {
                get { return id; }
            }
        }
    }
}
