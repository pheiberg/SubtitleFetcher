using System.Collections.Generic;

namespace SubtitleFetcher.Common.Downloaders.Addic7ed
{
    public interface ISubtitleMapper
    {
        Subtitle Map(Addic7edSubtitle subtitle, string title);

        IEnumerable<Subtitle> Map(IEnumerable<Addic7edSubtitle> subtitles, string title);
    }
}