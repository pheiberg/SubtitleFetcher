using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class Addic7edDownloader : ISubtitleDownloader
    {
        private readonly Addic7edScraper _scraper;

        public Addic7edDownloader()
        {
            _scraper = new Addic7edScraper();
        }

        public string GetName() => "Addic7ed";

        public IEnumerable<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            string fileName = Path.Combine(Path.GetTempPath(), subtitle.FileName);
            new WebDownloader().DownloadFile(subtitle.Id, fileName);
            return new[] { new FileInfo(fileName) };
        }

        public IEnumerable<Subtitle> SearchSubtitles(SearchQuery query)
        {
            int? seriesId = _scraper.FindSeries(query.SeriesTitle);
            if (!seriesId.HasValue)
                return Enumerable.Empty<Subtitle>();

            return _scraper.SearchSubtitles(seriesId.Value, query.SeriesTitle, query.Season, query.Episode, query.Languages);
        }
        
        public IEnumerable<Language> SupportedLanguages => new[]
        {
            KnownLanguages.GetLanguageByName("Arabic"),
            KnownLanguages.GetLanguageByName("English"),
            KnownLanguages.GetLanguageByName("French"),
            KnownLanguages.GetLanguageByName("German"),
            KnownLanguages.GetLanguageByName("Hungarian"),
            KnownLanguages.GetLanguageByName("Italian"),
            KnownLanguages.GetLanguageByName("Persian"),
            KnownLanguages.GetLanguageByName("Polish"),
            KnownLanguages.GetLanguageByName("Portuguese"),
            KnownLanguages.GetLanguageByName("Romanian"),
            KnownLanguages.GetLanguageByName("Russian"),
            KnownLanguages.GetLanguageByName("Spanish"),
            KnownLanguages.GetLanguageByName("Swedish")
        };

        public IEnumerable<IEnhancementRequest> EnhancementRequests => Enumerable.Empty<IEnhancementRequest>();
    }
}