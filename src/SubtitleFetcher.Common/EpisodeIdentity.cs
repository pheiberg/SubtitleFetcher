using System;

namespace SubtitleFetcher.Common
{
    public class EpisodeIdentity
    {
        public string SeriesName { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
        public int EndEpisode { get; set; }
        public string ReleaseGroup { get; set; }
        public bool IsMultiEpisode => EndEpisode > Episode;

        public EpisodeIdentity()
        {
            
        }

        public EpisodeIdentity(string seriesName, int season, int episode, int endEpisode, string releaseGroup)
        {
            SeriesName = seriesName;
            Season = season;
            Episode = episode;
            EndEpisode = endEpisode;
            ReleaseGroup = releaseGroup;
        }

        public bool Equals(EpisodeIdentity other)
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
            if (obj.GetType() != typeof (EpisodeIdentity)) return false;
            return Equals((EpisodeIdentity) obj);
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
            var name = SeriesName.Replace(" ", ".");
            var seasonNumber = Season.ToString("00");
            var episodeNumber = Episode.ToString("00");
            var endEpisodeNumber = EndEpisode.ToString("00");
            var episodeSuffix = IsMultiEpisode ? $"-E{endEpisodeNumber}" : "";
            return $"{name}.S{seasonNumber}E{episodeNumber}{episodeSuffix}-{ReleaseGroup}";
        }

        public bool IsEquivalent(EpisodeIdentity other)
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