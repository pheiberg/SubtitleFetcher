using System.Collections.Generic;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Orchestration
{
    public interface ISubtitleDownloadService
    {
        bool DownloadSubtitle(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<Language> languages);
    }
}