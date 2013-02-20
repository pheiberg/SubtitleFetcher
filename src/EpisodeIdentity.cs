using System;
using System.Text.RegularExpressions;

namespace SubtitleFetcher
{
    public class EpisodeIdentity
    {
        public string SeriesName { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public string ReleaseGroup { get; set; }

        public bool Equals(EpisodeIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(other.SeriesName, SeriesName, StringComparison.InvariantCultureIgnoreCase) && other.Season == Season && Equals(other.ReleaseGroup, ReleaseGroup) && other.Episode == Episode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (EpisodeIdentity)) return false;
            return Equals((EpisodeIdentity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (SeriesName != null ? SeriesName.GetHashCode() : 0);
                result = (result*397) ^ Season;
                result = (result*397) ^ (ReleaseGroup != null ? ReleaseGroup.GetHashCode() : 0);
                result = (result*397) ^ Episode;
                return result;
            }
        }

        public static bool operator ==(EpisodeIdentity left, EpisodeIdentity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EpisodeIdentity left, EpisodeIdentity right)
        {
            return !Equals(left, right);
        }
        public override string ToString()
        {
            return SeriesName + " - " + Season + " - " + Episode + " - " + ReleaseGroup;
        }

        public bool IsEquivalent(EpisodeIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return string.Equals(Normalize(other.SeriesName),  Normalize(SeriesName), StringComparison.InvariantCultureIgnoreCase) && other.Season == Season && Equals(other.ReleaseGroup, ReleaseGroup) && other.Episode == Episode;
        }

        private string Normalize(string name)
        {
            return Regex.Replace(name, "\\s", "");
        }
    }
}