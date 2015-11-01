using System;

namespace PortedSubtitleDownloaders.Legacy
{
    public class SearchQuery : SubtitleSearchQuery
    {
        private int? _year;

        public int? Year
        {
            get
            {
                return _year;
            }
            set
            {
                int? nullable1 = value;
                if ((nullable1.GetValueOrDefault() >= 1900 ? 0 : (nullable1.HasValue ? 1 : 0)) == 0)
                {
                    int? nullable2 = value;
                    if ((nullable2.GetValueOrDefault() <= 2050 ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
                    {
                        _year = value;
                        return;
                    }
                }
                throw new ArgumentException("Invalid year, must be between 1900-2050");
            }
        }

        public string Query { get; }

        public SearchQuery(string query)
        {
            Query = query;
        }
    }
}