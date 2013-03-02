namespace SubtitleFetcher.Common
{
    public interface ILogger
    {
        void Log(string source, string message, LogLevel level = LogLevel.Minimal);

        void Debug(string source, string message, params object[] args);

        void Verbose(string source, string message, params object[] args);

        void Important(string source, string message, params object[] args);

        void Error(string source, string message, params object[] args);
    }
}