using System;

namespace SubtitleFetcher
{
    public class Logger : ILogger
    {
        private readonly LogLevel level;

        public Logger(LogLevel level)
        {
            this.level = level;
        }

        public void Log(string message, LogLevel visibility)
        {
            if (visibility <= level)
                Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Log(message, LogLevel.Debug);
        }
    }

    public enum LogLevel
    {
        Minimal = 1,
        Verbose = 2,
        Debug = 3
    }
}