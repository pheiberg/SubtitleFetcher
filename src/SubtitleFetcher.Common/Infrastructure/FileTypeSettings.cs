using System.Collections.Generic;

namespace SubtitleFetcher.Common.Infrastructure
{
    public class FileTypeSettings
    {
        public FileTypeSettings(IEnumerable<string> acceptedExtensions)
        {
            AcceptedExtensions = acceptedExtensions;
        }

        public IEnumerable<string> AcceptedExtensions { get; }
    }
}