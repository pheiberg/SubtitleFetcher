using System.Collections.Generic;
using SubtitleFetcher.Common;

namespace SubtitleFetcher
{
    public interface IEpisodeSubtitleDownloader
    {
        IEnumerable<Subtitle> SearchSubtitle(TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages);

        bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile);

        bool CanHandleAtLeastOneOf(IEnumerable<string> languages);
    }
}