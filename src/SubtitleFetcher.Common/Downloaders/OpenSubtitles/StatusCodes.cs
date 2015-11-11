namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles
{
    public enum StatusCodes
    {
        None = 0,
        Ok = 200,
        PartialContent = 206,
        HostMoved = 301,
        Unauthorized = 401,
        InvalidSubtitleFormat = 402,
        SubHashesNotMatch = 403,
        InvalidSubtitleLanguage = 404,
        MissingParameters = 405,
        NoSession = 406,
        DownloadLimitReached = 407,
        InvalidParameters = 408,
        MethodNotFound = 409,
        UnknownError = 410,
        InvalidOrEmptyUserAgent = 411,
        InvalidFormat = 412,
        InvalidImdbId = 413,
        UnknownUserAgent = 414,
        DisabledUserAgent = 415,
        InternalSubtitleValidationFail = 416,
        ServiceUnavailable = 503
    }
}