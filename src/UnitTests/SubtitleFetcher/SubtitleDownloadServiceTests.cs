using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using SubtitleFetcher;
using SubtitleFetcher.Common;

namespace UnitTests.SubtitleFetcher
{
    [TestFixture]
    public class SubtitleDownloadServiceTests
    {
        [Test]
        public void DownloadSubtitle_NoMatchingSubtitles_ShouldReturnFalse()
        {
            var fakeDownloader = A.Fake<IEpisodeSubtitleDownloader>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(Enumerable.Empty<Subtitle>());
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders);

            bool success = service.DownloadSubtitle("Anything", new EpisodeIdentity(), new string[0]);

            Assert.That(success, Is.False);
        }

        [Test]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderSucceeds_ShouldReturnTrue()
        {
            var matches = new[] { new Subtitle("Anything", "Anything", "Anything", "eng") };
            var fakeDownloader = A.Fake<IEpisodeSubtitleDownloader>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders);

            bool success = service.DownloadSubtitle("Anything", new EpisodeIdentity(), new string[0]);

            Assert.That(success, Is.True);
        }
        
        [Test]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderFails_ShouldReturnFalse()
        {
            var matches = new[] { new Subtitle("Anything", "Anything", "Anything", "eng") };
            var fakeDownloader = A.Fake<IEpisodeSubtitleDownloader>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(false);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders);

            bool success = service.DownloadSubtitle("Anything", new EpisodeIdentity(), new string[0]);

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
            var fakeDownloader = A.Fake<IEpisodeSubtitleDownloader>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders);

            bool success = service.DownloadSubtitle("Target name", new EpisodeIdentity("", 1, 1, ""), languages);

            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>.That.Matches(s => s.LanguageCode == "swe"), "Target name"))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void DownloadSubtitle_SomeDowloadersCantHandleLanguages_OnlyCallsDownloadersThatHandlesLanguages()
        {
            var matches = new[] { 
                new Subtitle("Anything", "Anything", "Anything", "eng"), 
                new Subtitle("Anything", "Anything", "Anything", "swe"), 
            };
            var languages = new[] { "swe", "eng" };
            var fakeDownloader = A.Fake<IEpisodeSubtitleDownloader>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var incapableDownloader = A.Fake<IEpisodeSubtitleDownloader>();
            A.CallTo(() => incapableDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(false);
            var downloaders = new[] { fakeDownloader, incapableDownloader };
            var service = new SubtitleDownloadService(downloaders);

            bool success = service.DownloadSubtitle("Target name", new EpisodeIdentity("", 1, 1, ""), languages);

            A.CallTo(() => incapableDownloader.SearchSubtitle(A<EpisodeIdentity>._, A<string[]>._)).MustNotHaveHappened();
        }
    }
}
