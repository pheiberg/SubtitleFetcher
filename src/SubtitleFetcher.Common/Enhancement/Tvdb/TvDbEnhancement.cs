namespace SubtitleFetcher.Common.Enhancement.Tvdb
{
    public class TvDbEnhancement : IEnhancement
    {
        public TvDbEnhancement(int tvDbId)
        {
            TvDbId = tvDbId;
        }

        public int TvDbId { get;}
    }
}
