namespace SubtitleFetcher
{
    public interface ILogger
    {
        void Log(string format, params object[] parms);
    }
}