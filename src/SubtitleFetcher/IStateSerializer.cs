namespace SubtitleFetcher
{
    public interface IStateSerializer
    {
        SubtitleState LoadState();
        void SaveState(SubtitleState state);
    }
}