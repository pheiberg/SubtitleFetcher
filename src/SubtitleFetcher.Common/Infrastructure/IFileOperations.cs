using System.Collections.Generic;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Infrastructure
{
    public interface IFileOperations
    {
        IEnumerable<string> GetFilesToProcess(IEnumerable<string> fileLocations, IEnumerable<string> languages);
        bool HasDownloadedSubtitle(string filePath, IEnumerable<string> languages);
        void CreateNosrtFile(SubtitleStateEntry entry);
        IEnumerable<string> GetIgnoredShows(string ignoreFileName);
        void RenameSubtitleFile(string sourceFileName, string targetSubtitleFile);
        IEnumerable<Language> GetDowloadedSubtitleLanguages(string filePath, IEnumerable<Language> languages);
    }
}