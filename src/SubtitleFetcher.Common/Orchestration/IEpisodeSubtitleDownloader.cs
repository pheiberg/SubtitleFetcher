using System.Collections.Generic;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Orchestration
{
    public interface IEpisodeSubtitleDownloader
    {
        IEnumerable<Subtitle> SearchSubtitle(TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages);

        bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile);

        bool CanHandleAtLeastOneOf(IEnumerable<string> languages);

        IEnumerable<IEnhancementRequest> EnhancementRequests { get; } 
    }
}