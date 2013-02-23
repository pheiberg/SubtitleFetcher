using System;
using System.Collections.Generic;
using System.IO;

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

        public IEnumerable<string> GetIgnoredShows(string ignoreFileName)
        {
            if (ignoreFileName == null) 
                yield break;

            int count = 0;
            using (TextReader reader = new StreamReader(ignoreFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line.Trim();
                    count++;
                }
            }
            Log("Ignore shows file loaded. {0} shows ignored.", count);
        }
    }
}