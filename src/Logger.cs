using System;

namespace SubtitleFetcher
{
    public class Logger
    {
        private readonly bool verbose;

        public Logger(bool verbose)
        {
            this.verbose = verbose;
        }


        public void Log(string format, params object[] parms)
        {
            if (verbose)
                Console.WriteLine(String.Format(format, parms));
        }
    }
}