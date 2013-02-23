using System.Collections.Generic;

namespace SubtitleFetcher
{
    public interface IFileSystem
    {
        IEnumerable<string> GetFilesToProcess(IEnumerable<string> fileLocations);
        bool HasDownloadedSubtitle(string filePath);
        void CreateNosrtFile(SubtitleStateEntry entry);
        IEnumerable<string> GetIgnoredShows(string ignoreFileName);
        void RenameSubtitleFile(string targetSubtitleFile, string sourceFileName);
    }
}