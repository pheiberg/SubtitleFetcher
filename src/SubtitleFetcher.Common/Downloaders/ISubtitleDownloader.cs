using System.Collections.Generic;
using System.IO;

namespace SubtitleFetcher.Common.Downloaders
{
    public interface ISubtitleDownloader
    {
        IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle);
        string GetName();
        IEnumerable<Subtitle> SearchSubtitles(SearchQuery query);
        IEnumerable<string> LanguageLimitations { get; }
    }
}