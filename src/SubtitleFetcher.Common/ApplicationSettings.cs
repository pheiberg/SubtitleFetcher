using System.Configuration;

namespace SubtitleFetcher.Common
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}