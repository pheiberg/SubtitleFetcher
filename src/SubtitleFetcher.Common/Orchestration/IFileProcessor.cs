using System.Collections.Generic;

namespace SubtitleFetcher.Common.Orchestration
{
    public interface IFileProcessor
    {
        bool ProcessFile(string fileName, IEnumerable<string> ignoredShows);
    }
}