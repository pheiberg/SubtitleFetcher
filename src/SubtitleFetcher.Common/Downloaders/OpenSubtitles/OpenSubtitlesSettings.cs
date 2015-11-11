namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles
{
    public class OpenSubtitlesSettings
    {
        public string Url { get; set; }
        public int Timeout { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public string UserAgent { get; set; }
    }
}