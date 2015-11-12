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
            var enhancementRequests = _downloaders.SelectMany(d => d.EnhancementRequests);
            var enhancements = enhancementRequests.Select(
                er => _enhancementProvider.GetEnhancement(er.EnhancementType, filePath, identity))
                .Where(enhancement => enhancement != null);

            identity.Enhancements.AddRange(enhancements);
        }
    }
}