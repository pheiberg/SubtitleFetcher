using System;
using System.Net;

namespace SubtitleFetcher.Common.Downloaders.SubDb
{
    public class SubDbHttpException : Exception
    {
        public SubDbHttpException(HttpStatusCode statusCode) 
            : base("Subtitle request failed with code " + statusCode)
        {

        }
    }
}