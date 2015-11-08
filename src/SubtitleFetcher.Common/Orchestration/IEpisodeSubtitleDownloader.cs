using System.Collections.Generic;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

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