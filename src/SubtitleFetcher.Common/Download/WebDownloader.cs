using System;
using System.Net;

namespace SubtitleFetcher.Common.Download
{
    public class WebDownloader : WebClient
    {

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout
        {
            get;
            set;
        }

        public WebDownloader()
            : this(60000)
        {

        }

        public WebDownloader(int timeout)
        {
            Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var result = base.GetWebRequest(address);
            if (result != null)
            {
                result.Timeout = Timeout;
            }
            return result;
        }
    }
}