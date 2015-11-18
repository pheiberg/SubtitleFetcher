using System.Configuration;
using SubtitleFetcher.Common;

namespace SubtitleFetcher.Settings
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}