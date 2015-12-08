using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class Addic7edDownloader : ISubtitleDownloader
    {
        private readonly IAddic7edScraper _scraper;
        private readonly ISubtitleFilter _filter;
        private readonly ISubtitleMapper _mapper;

        public Addic7edDownloader() : this(
            new Addic7edScraper(), 
            new SubtitleFilter(), 
            new SubtitleMapper())
        {

        }

        public Addic7edDownloader(
            IAddic7edScraper scraper, 
            ISubtitleFilter filter,
            ISubtitleMapper mapper)
        {
            _scraper = scraper;
            _filter = filter;
            _mapper = mapper;
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

            var listedSubtitles = _scraper.SearchSubtitles(seriesId.Value, query.Season);
            var subtitles = _mapper.Map(listedSubtitles, query.SeriesTitle);
            return _filter.Apply(subtitles, query);
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