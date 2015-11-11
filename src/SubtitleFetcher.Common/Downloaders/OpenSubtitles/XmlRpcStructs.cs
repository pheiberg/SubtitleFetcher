using CookComputing.XmlRpc;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles
{
    public class ResponseBase
    {
        public string status;
        public double seconds;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class LoginResponse : ResponseBase
    {
        public string token;
    }

    public class SearchSubtitlesRequest
    {
        public string sublanguageid = string.Empty;
        public string moviehash = string.Empty;
        public string moviebytesize = string.Empty;
        public string imdbid = string.Empty;
        public string query = string.Empty;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int? season;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public int? episode;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class SearchSubtitlesResponse : ResponseBase
    {
        public SearchSubtitlesInfo[] data;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class SearchSubtitlesInfo
    {
        public string MatchedBy;
        public string IDSubMovieFile;
        public string MovieHash;
        public string MovieByteSize;
        public string MovieTimeMS;
        public string IDSubtitleFile;
        public string SubFileName;
        public string SubActualCD;
        public string SubSize;
        public string SubHash;
        public string IDSubtitle;
        public string UserID;
        public string SubLanguageID;
        public string SubFormat;
        public string SubSumCD;
        public string SubAuthorComment;
        public string SubAddDate;
        public string SubBad;
        public string SubRating;
        public string SubDownloadsCnt;
        public string MovieReleaseName;
        public string IDMovie;
        public string IDMovieImdb;
        public string MovieName;
        public string MovieNameEng;
        public string MovieYear;
        public string MovieImdbRating;
        public string SubFeatured;
        public string UserNickName;
        public string ISO639;
        public string LanguageName;
        public string SubComments;
        public string SubHearingImpaired;
        public string UserRank;
        public string SubDownloadLink;
        public string ZipDownloadLink;
        public string SubtitlesLink;
        public string MovieFPS;
        public string MovieKind;
        public int QueryNumber;
        public string SeriesEpisode;
        public string SeriesIMDBParent;
        public string SeriesSeason;
        public string SubEncoding;
        public string SubHD;
        public string SubLastTS;
    }

    public enum OpenSubtitlesKind
    {
        None = 0,
        Movie = 1,
        Episode = 2
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class GetSubLanguagesResponse : ResponseBase
    {
        public GetSubLanguagesInfo[] data;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class GetSubLanguagesInfo
    {
        public string SubLanguageID;
        public string LanguageName;
        public string ISO639;
    }
}
