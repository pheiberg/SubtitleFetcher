using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace SubtitleFetcher.Common.Downloaders.SubDb
{
    public class SubDbApi
    {
        private const string UserAgent = "SubDB/1.0 (SubtitleFetcher/0.9; http://github.com/pheiberg/SubtitleFetcher)";
        private const string BaseUri = "http://api.thesubdb.com";
        
        public FileInfo DownloadSubtitle(string hash, IEnumerable<string> languages)
        {
            string tempFileName = Path.GetTempFileName();
            var language = string.Join(",", languages);
            var action = $"?action=download&hash={hash}&lang={language}";

            using (var client = CreateHttpClient())
            using (var responseStream = FetchFile(client, action))
            using (var fileStream = File.OpenWrite(tempFileName))
            {
                responseStream.CopyTo(fileStream);
                fileStream.Flush();
                return new FileInfo(tempFileName);
            }
        }

        private static Stream FetchFile(HttpClient client, string action)
        {
            var httpResponseMessage = client.GetAsync(action).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
                return httpResponseMessage.Content.ReadAsStreamAsync().Result;

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                throw new SubDbFileNotFoundException();

            throw new SubDbHttpException(httpResponseMessage.StatusCode);
        }

        public IEnumerable<string> Search(string hash)
        {
            string action = $"?action=search&hash={hash}";
            using (var client = CreateHttpClient())
            {
                var httpResponseMessage = client.GetAsync(action).Result;
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var result = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    return result.Split(',').Select(r => r.Trim());
                }

                if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                    return Enumerable.Empty<string>();

                throw new SubDbHttpException(httpResponseMessage.StatusCode);
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
            httpClient.BaseAddress = new Uri(BaseUri);
            return httpClient;
        }
    }
}