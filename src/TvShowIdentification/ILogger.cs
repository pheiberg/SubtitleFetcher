namespace TvShowIdentification
{
    public interface ILogger
    {
        void Log(string message, LogLevel level = LogLevel.Minimal);

        void Debug(string message, params object[] args);

        void Verbose(string message, params object[] args);

        void Important(string message, params object[] args);

        void Error(string message, params object[] args);
    }
}