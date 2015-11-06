using System;
using System.Collections.Generic;

namespace SubtitleFetcher.Common
{
    public class TvReleaseIdentity
    {
        public string SeriesName { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public int EndEpisode { get; set; }
        public string ReleaseGroup { get; set; }
        public ICollection<string> Tags { get; private set; } 
        public bool IsMultiEpisode => EndEpisode > Episode;
        public string FileHash { get; set; }

        public TvReleaseIdentity()
        {
            Tags = new List<string>();
        }

        public TvReleaseIdentity(string seriesName, int season, int episode, int endEpisode, string releaseGroup, string fileHash)
        {
            SeriesName = seriesName;
            Season = season;
            Episode = episode;
            EndEpisode = endEpisode;
            ReleaseGroup = releaseGroup;
            FileHash = fileHash;
        }

        public bool Equals(TvReleaseIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(other.SeriesName, SeriesName, StringComparison.OrdinalIgnoreCase) 
                && other.Season == Season 
                && string.Equals(other.ReleaseGroup, ReleaseGroup, StringComparison.OrdinalIgnoreCase) 
                && other.Episode == Episode && other.EndEpisode == EndEpisode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TvReleaseIdentity)) return false;
            return Equals((TvReleaseIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = SeriesName?.GetHashCode() ?? 0;
                result = (result*397) ^ Season;
                result = (result*397) ^ (ReleaseGroup?.GetHashCode() ?? 0);
                result = (result*397) ^ Episode;
                result = (result*397) ^ EndEpisode;
                return result;
            }
        }

        public static bool operator ==(TvReleaseIdentity left, TvReleaseIdentity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TvReleaseIdentity left, TvReleaseIdentity right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var name = SeriesName.Replace(" ", ".");
            var seasonNumber = Season.ToString("00");
            var episodeNumber = Episode.ToString("00");
            var endEpisodeNumber = EndEpisode.ToString("00");
            var episodeSuffix = IsMultiEpisode ? $"-E{endEpisodeNumber}" : "";
            return $"{name}.S{seasonNumber}E{episodeNumber}{episodeSuffix}-{ReleaseGroup}";
        }

        public bool IsEquivalent(TvReleaseIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (SeriesName == null || other.SeriesName == null) return Equals(other);
            
            return string.Equals(other.SeriesName.RemoveNonAlphaNumericChars(),  SeriesName.RemoveNonAlphaNumericChars(), StringComparison.OrdinalIgnoreCase) 
                && other.Season == Season 
                && string.Equals(other.ReleaseGroup, ReleaseGroup, StringComparison.OrdinalIgnoreCase) 
                && other.Episode == Episode && other.EndEpisode == EndEpisode;
        }
    }
}