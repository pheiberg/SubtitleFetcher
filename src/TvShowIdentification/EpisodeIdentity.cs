using System;

namespace TvShowIdentification
{
    public class EpisodeIdentity
    {
        public string SeriesName { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public string ReleaseGroup { get; set; }

        public EpisodeIdentity()
        {
            
        }

        public EpisodeIdentity(string seriesName, int season, int episode, string releaseGroup)
        {
            SeriesName = seriesName;
            Season = season;
            Episode = episode;
            ReleaseGroup = releaseGroup;
        }

        public bool Equals(EpisodeIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(other.SeriesName, SeriesName, StringComparison.OrdinalIgnoreCase) && other.Season == Season && string.Equals(other.ReleaseGroup, ReleaseGroup, StringComparison.OrdinalIgnoreCase) && other.Episode == Episode;
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
            return SeriesName.Replace(" ", ".") + ".S" + Season.ToString("00") + "E" + Episode.ToString("00") + "-" + ReleaseGroup;
        }

        public bool IsEquivalent(EpisodeIdentity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (SeriesName == null || other.SeriesName == null) return Equals(other);
            
            return string.Equals(other.SeriesName.RemoveNonAlphaNumericChars(),  SeriesName.RemoveNonAlphaNumericChars(), StringComparison.OrdinalIgnoreCase) 
                && other.Season == Season && string.Equals(other.ReleaseGroup, ReleaseGroup, StringComparison.OrdinalIgnoreCase) && other.Episode == Episode;
        }
    }
}