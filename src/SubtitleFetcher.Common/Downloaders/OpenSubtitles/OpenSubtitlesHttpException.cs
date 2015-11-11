using System;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles
{
    public class OpenSubtitlesHttpException : Exception
    {
        public OpenSubtitlesHttpException(string message) : base(message)
        {
        }
    }
}