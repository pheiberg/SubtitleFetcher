using System.Collections.Generic;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Orchestration
{
    public interface IEpisodeSubtitleDownloader
    {
        IEnumerable<Subtitle> SearchSubtitle(TvReleaseIdentity tvReleaseIdentity, IEnumerable<Language> languages);

        bool TryDownloadSubtitle(Subtitle subtitle, string targetSubtitleFile);

        bool CanHandleAtLeastOneOf(IEnumerable<Language> languages);

        IEnumerable<IEnhancementRequest> EnhancementRequests { get; } 
    }
}