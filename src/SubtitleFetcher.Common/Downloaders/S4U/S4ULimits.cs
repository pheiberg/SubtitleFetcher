using System.Collections.Generic;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class S4ULimits
    {
        public S4ULimits()
        {
            Custom = new Dictionary<string, string>();
        }

        public int? Year { get; set; }

        public int? Season { get; set; }

        public int? Episode { get; set; }

        public int? Limit { get; set; }

        public IDictionary<string, string> Custom { get; private set; }
    }
}