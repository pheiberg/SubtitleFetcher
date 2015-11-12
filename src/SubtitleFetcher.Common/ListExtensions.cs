using System;
using System.Collections.Generic;

namespace SubtitleFetcher.Common
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> values)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var concreteList = list as List<T>;
            if(concreteList != null)
            {
                concreteList.AddRange(values);
            }
            else
            {
                foreach (var value in values)
                {
                    list.Add(value);
                }
            }
        }
    }
}
