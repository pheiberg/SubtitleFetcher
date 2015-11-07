using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitleFetcher.Common.Enhancement
{
    public class EnhancerRegistry
    {
        public EnhancerRegistry(IEnumerable<IEnhancer> enhancers)
        {
            Enhancers = new List<IEnhancer>(enhancers);
        }

        public IList<IEnhancer> Enhancers { get; }

        public IEnhancer GetEnhancerFor<T>() where T : IEnhancement
        {
            return GetEnhancerFor(typeof (T));
        }

        public IEnhancer GetEnhancerFor(Type enhancementType)
        {
            return Enhancers.FirstOrDefault(e => e.ProvidedEnhancement == enhancementType);
        }
    }
}