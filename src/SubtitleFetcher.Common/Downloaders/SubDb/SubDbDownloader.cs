using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using SubtitleFetcher.Common.Download;

namespace SubtitleFetcher.Common.Downloaders.SubDb
{
    public class SubDbDownloader : ISubtitleDownloader
    {
        const string UserAgent = "SubDB/1.0 (SubtitleFetcher/0.9; http://github.com/pheiberg/SubtitleFetcher)";
        private const string BaseUri = "http://sandbox.thesubdb.com";
        private readonly Dictionary<string, string> LanguageLookup = new Dictionary<string, string>
        {
            {"eng", "en"},
            {"spa", "es"},
            {"fre", "fr"},
            {"ita", "it"},
            {"dut", "nl"},
            {"pol", "pl"},
            {"por", "pt"},
            {"rom", "ro"},
            {"swe", "sv"},
            {"tur", "tr"}
        };

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            throw new NotImplementedException();
        }

        public FileInfo DownloadSubtitle(string hash, IEnumerable<string> languages)
        {
            string tempFileName = Path.GetTempFileName();
            var language = string.Join(",", languages);
            var action = $"?action=download&hash={hash}&lang={language}";

            using (var client = CreateHttpClient())
            using (var responseStream = client.GetStreamAsync(action).Result)
            using (var fileStream = File.OpenWrite(tempFileName))
            {
                responseStream.CopyTo(fileStream);
                fileStream.Flush();
                return new FileInfo(tempFileName);
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            httpClient.BaseAddress = new Uri(BaseUri);
            return httpClient;
        }


        public string GetName()
        {
            return "SubDb";
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LanguageLimitations => new[] { "swe", "eng" };
    }
}