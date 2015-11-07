using System.Collections.Generic;

namespace SubtitleFetcher.Common.Orchestration
{
    public interface ISubtitleDownloadService
    {
        bool DownloadSubtitle(string targetSubtitleFile, TvReleaseIdentity tvReleaseIdentity, IEnumerable<string> languages);
    }
}