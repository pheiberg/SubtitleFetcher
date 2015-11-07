namespace SubtitleFetcher.Common.Infrastructure
{
    public interface IStateSerializer
    {
        SubtitleState LoadState();
        void SaveState(SubtitleState state);
    }
}