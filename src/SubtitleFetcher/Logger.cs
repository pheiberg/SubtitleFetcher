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

        public void Error(string message, params object[] args)
        {
            Console.Error.WriteLine(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            Log(string.Format(message, args), LogLevel.Debug);
        }
        
        public void Verbose(string message, params object[] args)
        {
            Log(string.Format(message, args), LogLevel.Verbose);
        }
        
        public void Important(string message, params object[] args)
        {
            Log(string.Format(message, args), LogLevel.Minimal);
        }
    }

    public enum LogLevel
    {
        Minimal = 1,
        Verbose = 2,
        Debug = 3
    }
}