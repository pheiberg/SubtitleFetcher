using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Common.Languages;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public class SubtitleMapper : ISubtitleMapper
    {
        public Subtitle Map(Addic7edSubtitle subtitle, string title)
        {
            return new Subtitle(subtitle.DowloadLink,
                $"{title}.S{subtitle.Season.ToString("00")}.E{subtitle.Episode.ToString("00")}.DUMMY-{subtitle.Version}",
                KnownLanguages.GetLanguageByName(subtitle.Language))
            {
                SeriesName = title,
                Season = subtitle.Season,
                Episode = subtitle.Episode,
                EndEpisode = subtitle.Episode,
                ReleaseGroup = subtitle.Version
            };
        }

        public IEnumerable<Subtitle> Map(IEnumerable<Addic7edSubtitle> subtitles, string title)
        {
            return subtitles.Select(s => Map(s, title));
        } 
    }
}
