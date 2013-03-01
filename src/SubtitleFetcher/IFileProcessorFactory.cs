using System.Collections.Generic;

namespace SubtitleFetcher
{
    public interface IFileProcessorFactory
    {
        IFileProcessor Create(IEnumerable<string> ignoredShows);
    }
}