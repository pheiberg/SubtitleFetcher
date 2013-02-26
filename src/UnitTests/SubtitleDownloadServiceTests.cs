using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using SubtitleDownloader.Core;
using SubtitleFetcher;

namespace UnitTests
{
    [TestFixture]
    public class SubtitleDownloadServiceTests
    {
        [Test]
        public void DownloadSubtitle_NoMatchingSubtitles_ShouldReturnFalse()
        {

            var fakeDownloader = A.Fake<ISubtitleDownloadProvider>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(Enumerable.Empty<Subtitle>());
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders, new string[0]);

            bool success = service.DownloadSubtitle("Anything", new EpisodeIdentity());

            Assert.That(success, Is.False);
        }

        [Test]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderSucceeds_ShouldReturnTrue()
        {
            var matches = new[] { new Subtitle("Anything", "Anything", "Anything", "eng") };
            var fakeDownloader = A.Fake<ISubtitleDownloadProvider>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders, new string[0]);

            bool success = service.DownloadSubtitle("Anything", new EpisodeIdentity());

            Assert.That(success, Is.True);
        }
        
        [Test]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderFails_ShouldReturnFalse()
        {
            var matches = new[] { new Subtitle("Anything", "Anything", "Anything", "eng") };
            var fakeDownloader = A.Fake<ISubtitleDownloadProvider>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(false);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders, new string[0]);

            bool success = service.DownloadSubtitle("Anything", new EpisodeIdentity());

            Assert.That(success, Is.False);
        }

        [Test]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderSucceeds_DownloaderShouldDownloadFirstLanguage()
        {
            var matches = new[] { 
                new Subtitle("Anything", "Anything", "Anything", "eng"), 
                new Subtitle("Anything", "Anything", "Anything", "swe"), 
            };
            var languages = new[] {"swe", "eng"};
            var fakeDownloader = A.Fake<ISubtitleDownloadProvider>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders, languages);

            bool success = service.DownloadSubtitle("Target name", new EpisodeIdentity("", 1, 1, ""));

            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>.That.Matches(s => s.LanguageCode == "swe"), "Target name"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
