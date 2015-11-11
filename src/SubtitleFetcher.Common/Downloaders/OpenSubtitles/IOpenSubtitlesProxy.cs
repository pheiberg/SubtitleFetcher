using CookComputing.XmlRpc;

namespace SubtitleFetcher.Common.Downloaders.OpenSubtitles
{
    public interface IOpenSubtitlesProxy : IXmlRpcProxy
    {
        [XmlRpcMethod("LogIn")]
        LoginResponse Login(string username, string password, string language, string useragent);
        
        [XmlRpcMethod("SearchSubtitles")]
        SearchSubtitlesResponse SearchSubtitles(string token, SearchSubtitlesRequest[] request);
        
        [XmlRpcMethod("GetSubLanguages")]
        GetSubLanguagesResponse GetSubLanguages(string language);
    }
}