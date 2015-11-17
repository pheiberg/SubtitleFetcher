using System.Collections.Generic;
using System.Linq;

namespace SubtitleFetcher.Common.Downloaders.S4U
{
    public class LimitsBuilder
    {
        public string BuildString(S4ULimits limits)
        {
            var entries = limits.Custom.Select(extra => new KeyValuePair<string, string>(extra.Key, extra.Value)).ToList();

            if (limits.Season.HasValue)
            {
                entries.Add(new KeyValuePair<string, string>("Season", limits.Season.Value.ToString()));
            }

            if(limits.Episode.HasValue)
            {
                entries.Add(new KeyValuePair<string, string>("Episode", limits.Episode.Value.ToString()));
            }

            if (limits.Limit.HasValue)
            {
                entries.Add(new KeyValuePair<string, string>("Limit", limits.Limit.Value.ToString()));
            }

            if (limits.Year.HasValue)
            {
                entries.Add(new KeyValuePair<string, string>("Year", limits.Year.Value.ToString()));
            }

            if (!entries.Any())
                return string.Empty;

            var parameters = entries.Select(e => $"{e.Key}={e.Value}");
            return "/" + string.Join("&", parameters);
        }
    }
}