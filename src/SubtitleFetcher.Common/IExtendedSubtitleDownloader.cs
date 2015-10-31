using System.Collections.Generic;

namespace SubtitleFetcher.Common
{
    public interface IExtendedSubtitleDownloader : ISubtitleDownloader, IDownloadCapabilitiesProvider
    {
        IEnumerable<string> LanguageLimitations { get; }
    }
}
