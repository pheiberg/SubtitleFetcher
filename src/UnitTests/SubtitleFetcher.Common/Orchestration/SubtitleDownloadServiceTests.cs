using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders.SubDb.Enhancement;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Orchestration;

namespace UnitTests.SubtitleFetcher.Common.Orchestration
{
    [TestFixture]
    public class SubtitleDownloadServiceTests
    {
        [Test, AutoFakeData]
        public void DownloadSubtitle_NoMatchingSubtitles_ShouldReturnFalse(
            Language[] languages,
            string file,
            TvReleaseIdentity identity,
            IEnhancementProvider enhancementProvider,
            [Frozen]IEpisodeSubtitleDownloader fakeDownloader)
        {
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<Language[]>._)).Returns(Enumerable.Empty<Subtitle>());
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<Language[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders, enhancementProvider);

            bool success = service.DownloadSubtitle(file, identity, languages);

            Assert.That(success, Is.False);
        }

        [Test, AutoFakeData]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderSucceeds_ShouldReturnTrue(
            string file,
            TvReleaseIdentity identity,
            Subtitle[] matches,
            IEpisodeSubtitleDownloader fakeDownloader,
            IEnhancementProvider enhancementProvider
            )
        {
            var languages = matches.Select(m => m.Language);
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<Language[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<Language[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders, enhancementProvider);

            bool success = service.DownloadSubtitle(file, identity, languages);

            Assert.That(success, Is.True);
        }
        
        [Test, AutoFakeData]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderFails_ShouldReturnFalse(
            string file,
            Language[] languages,
            TvReleaseIdentity identity,
            Subtitle[] matches,
            Language language,
            IEnhancementProvider enhancementProvider,
            [Frozen]IEpisodeSubtitleDownloader fakeDownloader)
        {
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<Language[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(false);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<Language[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders, enhancementProvider);

            bool success = service.DownloadSubtitle(file, identity, languages);

            Assert.That(success, Is.False);
        }

        [Test, AutoFakeData]
        public void DownloadSubtitle_MatchingSubtitlesDownloaderSucceeds_DownloaderShouldDownloadFirstLanguage(
            string file,
            Subtitle[] matches,
            TvReleaseIdentity identity,
            IEpisodeSubtitleDownloader fakeDownloader,
            IEnhancementProvider enhancementProvider
            )
        {
            var languages = matches.Skip(1).Select(m => m.Language);
            var expectedLanguage = languages.First();
            A.CallTo(() => fakeDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<Language[]>._)).Returns(matches);
            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => fakeDownloader.CanHandleAtLeastOneOf(A<Language[]>._)).Returns(true);
            var downloaders = new[] { fakeDownloader };
            var service = new SubtitleDownloadService(downloaders, enhancementProvider);

            service.DownloadSubtitle(file, identity, languages);

            A.CallTo(() => fakeDownloader.TryDownloadSubtitle(A<Subtitle>.That.Matches(s => s.Language == expectedLanguage), A<string>._))
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
            A.CallTo(() => expectedDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<Language[]>._)).Returns(matches);
            A.CallTo(() => expectedDownloader.TryDownloadSubtitle(A<Subtitle>._, A<string>._)).Returns(true);
            A.CallTo(() => expectedDownloader.CanHandleAtLeastOneOf(A<Language[]>._)).Returns(true);

            var incapableDownloaders = downloaders.Skip(1).ToArray();
            incapableDownloaders.ForEach(incapableDownloader =>
                A.CallTo(() => incapableDownloader.CanHandleAtLeastOneOf(A<Language[]>._)).Returns(false)
            );
            
            var languages = matches.Select(m => m.Language);
            
            sut.DownloadSubtitle(file, identity, languages);

            incapableDownloaders.ForEach(incapableDownloader =>
                A.CallTo(() => incapableDownloader.SearchSubtitle(A<TvReleaseIdentity>._, A<Language[]>._))
                    .MustNotHaveHappened()
            );
        }

        [Test, AutoFakeData]
        public void DownloadSubtitle_DownloaderHasEnhancementRequests_ProvidesEnhancements(
            SubDbFileHashEnhancement enhancement,
            string file,
            TvReleaseIdentity identity,
            Subtitle[] matches,
            [Frozen]IEnumerable<IEpisodeSubtitleDownloader> downloaders,
            [Frozen]IEnhancementProvider enhancementProvider,
            SubtitleDownloadService sut
            )
        {
            var languages = matches.Select(m => m.Language);
            var downloaderWithRequest = downloaders.First();
            A.CallTo(() => downloaderWithRequest.CanHandleAtLeastOneOf(A<Language[]>._)).Returns(true);
            A.CallTo(() => downloaderWithRequest.EnhancementRequests)
                .Returns(new[] {new EnhancementRequest<SubDbFileHashEnhancement>()});
            A.CallTo(() => enhancementProvider.GetEnhancement(typeof(SubDbFileHashEnhancement), file, identity))
                .Returns(enhancement);
            
            sut.DownloadSubtitle(file, identity, languages);

            A.CallTo(() => downloaderWithRequest.SearchSubtitle(
                A<TvReleaseIdentity>.That.Matches(r => r.Enhancements.Contains(enhancement)), 
                A<IEnumerable<Language>>._)).MustHaveHappened();
        }
    }
}
