using System.Linq;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class ReleaseGroupParser
    {
        public string Parse(string release)
        {
            if (release == null)
                return null;

            var parts = release.Split(' ', '.', '-');
            return parts.LastOrDefault();
        }
    }
}
