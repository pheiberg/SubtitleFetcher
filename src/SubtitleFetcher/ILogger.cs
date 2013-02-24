namespace SubtitleFetcher
{
    public interface ILogger
    {
        void Log(string message, LogLevel level = LogLevel.Minimal);
        void Debug(string message);
    }
}