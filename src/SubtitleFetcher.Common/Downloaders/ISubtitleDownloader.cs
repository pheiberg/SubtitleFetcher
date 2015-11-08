using System.Collections.Generic;
using System.IO;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders
{
    public interface ISubtitleDownloader
    {
        IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle);
        string GetName();
        IEnumerable<Subtitle> SearchSubtitles(SearchQuery query);
        IEnumerable<Language> SupportedLanguages { get; }
        IEnumerable<IEnhancementRequest> EnhancementRequests { get; }   
    }
}