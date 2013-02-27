using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubtitleDownloader.Core;

namespace SwesubDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var d = new SwesubDownloader();
            var results = d.SearchSubtitles(new ImdbSearchQuery("0460681")).ToList();
            foreach (var subtitle in results)
            {
                Console.WriteLine(subtitle.FileName);
            }

            d.SaveSubtitle(results.Last());
        }
    }
}
