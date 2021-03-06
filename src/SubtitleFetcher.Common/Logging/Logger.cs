using System;

namespace SubtitleFetcher.Common.Logging
{
    public class Logger : ILogger
    {
        private readonly LogLevel level;

        public Logger(LogLevel level)
        {
            this.level = level;
        }

        public void Log(string source, string message, LogLevel visibility)
        {
            if (visibility <= level)
                Console.WriteLine("{{{0}}} - {1}", source, message);
        }

        public void Error(string source, string message, params object[] args)
        {
            Console.Error.WriteLine(message, args);
        }

        public void Debug(string source, string message, params object[] args)
        {
            Log(source, string.Format(message, args), LogLevel.Debug);
        }

        public void Verbose(string source, string message, params object[] args)
        {
            Log(source, string.Format(message, args), LogLevel.Verbose);
        }

        public void Important(string source, string message, params object[] args)
        {
            Log(source, string.Format(message, args), LogLevel.Minimal);
        }
    }

    public enum LogLevel
    {
        Minimal = 1,
        Verbose = 2,
        Debug = 3
    }
}