using System.Collections.Generic;
using System.IO;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles
{
    public class OpenSubtitlesDownloader : ISubtitleDownloader
    {
        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            yield break;
        }

        public string GetName()
        {
            return "OpenSubtitles";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            yield break;
        }

        public IEnumerable<Language> SupportedLanguages => KnownLanguages.AllLanguages;
        public IEnumerable<IEnhancementRequest> EnhancementRequests { get; }
    }
}
