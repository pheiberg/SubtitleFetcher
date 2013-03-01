using System.Collections.Generic;

namespace SubtitleFetcher
{
    public interface IFileProcessor
    {
        bool ProcessFile(string fileName, IEnumerable<string> ignoredShows);
    }
}