using System.Linq;
using Castle.Core.Internal;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher;
using SubtitleFetcher.Common;

namespace UnitTests.SubtitleFetcher
{
    [TestFixture]
    public class SubtitleDownloadServiceTests
    {
        [Test, AutoFakeData]
        public void DownloadSubtitle_NoMatchingSubtitles_ShouldReturnFalse(
            [Frozen]IEpisodeSubtitleDownloader fakeDownloader)
        {
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<string[]>._)).Returns(Enumerable.Empty<Subtitle>());
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders);

            bool success = service.DownloadSubtitle("Anything", new TvReleaseIdentity(), new string[0]);

            Assert.That(success, Is.False);
        }

        [Test, AutoFakeData]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderSucceeds_ShouldReturnTrue(
            string file,
            TvReleaseIdentity identity,
            string[] languages,
            Subtitle[] matches,
            IEpisodeSubtitleDownloader fakeDownloader
            )
        {
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders);

            bool success = service.DownloadSubtitle(file, identity, languages);

            Assert.That(success, Is.True);
        }
        
        [Test]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderFails_ShouldReturnFalse()
        {
            var matches = new[] { new Subtitle("Anything", "Anything", "Anything", "eng") };
            var fakeDownloader = A.Fake<IEpisodeSubtitleDownloader>();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(false);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders);

            bool success = service.DownloadSubtitle("Anything", new TvReleaseIdentity(), new string[0]);

            Assert.That(success, Is.False);
        }

        [Test, AutoFakeData]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderSucceeds_DownloaderShouldDownloadFirstLanguage(
            string file,
            Subtitle[] matches,
            TvReleaseIdentity identity,
            IEpisodeSubtitleDownloader fakeDownloader
            )
        {
            var languages = matches.Skip(1).Select(m => m.LanguageCode);
            var expectedLanguage = languages.First();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders);

            service.DownloadSubtitle(file, identity, languages);

            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>.That.Matches(s => s.LanguageCode == expectedLanguage), A<string>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test, AutoFakeData]
        public void DownloadSubtitle_SomeDowloadersCantHandleLanguages_OnlyCallsDownloadersThatHandlesLanguages(
            string file,
            TvReleaseIdentity identity,
            Subtitle[] matches,
            [Frozen]IEpisodeSubtitleDownloader[] downloaders,
            SubtitleDownloadService sut
            )
        {
            var expectedDownloader = downloaders.First();
            A.CallTo(() => expectedDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<string[]>._)).Returns(matches);
            A.CallTo(() => expectedDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => expectedDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(true);

            var incapableDownloaders = downloaders.Skip(1).ToArray();
            incapableDownloaders.ForEach(incapableDownloader =>
                A.CallTo(() => incapableDownloader.CanHandleAtLeastOneOf(A<string[]>._)).Returns(false)
            );
            
            var languages = matches.Select(m => m.LanguageCode);
            
            sut.DownloadSubtitle(file, identity, languages);

            incapableDownloaders.ForEach(incapableDownloader =>
                A.CallTo(() => incapableDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<string[]>._))
                    .MustNotHaveHappened()
            );
        }
    }
}
