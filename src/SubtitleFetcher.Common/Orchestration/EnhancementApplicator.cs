using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Common.Enhancement;

namespace SubtitleFetcher.Common.Orchestration
{
    public class EnhancementApplicator
    {
        private readonly IEnumerable<IEpisodeSubtitleDownloader> _downloaders;
        private readonly IEnhancementProvider _enhancementProvider;

        public EnhancementApplicator(IEnumerable<IEpisodeSubtitleDownloader>  downloaders, IEnhancementProvider enhancementProvider)
        {
            _downloaders = downloaders;
            _enhancementProvider = enhancementProvider;
        }

        public void ApplyEnhancements(string filePath, TvReleaseIdentity identity)
        {
            Type[] enhancementRequests = GetDistinctEnhancementRequestsFromDownloaders();
            var enhancements = GetEnhancements(filePath, identity, enhancementRequests);

            identity.Enhancements.AddRange(enhancements);
        }

        private Type[] GetDistinctEnhancementRequestsFromDownloaders()
        {
            return _downloaders.SelectMany(d => d.EnhancementRequests)
                .Select(er => er.EnhancementType)
                .Distinct().ToArray();
        }

        private IEnumerable<IEnhancement> GetEnhancements(string filePath, TvReleaseIdentity identity, Type[] enhancementTypes)
        {
            return enhancementTypes.Select(
                enhancementType => _enhancementProvider.GetEnhancement(enhancementType, filePath, identity))
                .Where(enhancement => enhancement != null);
        }
    }
}