using System.Collections.Generic;
using SubtitleDownloader.Core;

namespace SubtitleFetcher.Common
{
    public interface IExtendedSubtitleDownloader : ISubtitleDownloader, IDownloadCapabilitiesProvider
    {
        IEnumerable<string> LanguageLimitations { get; }
    }
}
