using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;
using SubtitleFetcher.Common;
using SubtitleFetcher.Common.Downloaders.SubDb.Enhancement;
using SubtitleFetcher.Common.Enhancement;
using SubtitleFetcher.Common.Enhancement.Tvdb;
using SubtitleFetcher.Common.Orchestration;

namespace UnitTests.SubtitleFetcher.Common.Orchestration
{
    [TestFixture]
    public class EnhancementApplicatorTests
    {
        [Test, AutoFakeData]
        public void ApplyEnhancements_EnhancementRequestsMade_ResultsAddedToIdentity(
            string filePath,
            TvReleaseIdentity identity,
            IEnumerable<IEnhancementRequest> enhancementRequests,
            IEnhancement[] enhancements,
            IEpisodeSubtitleDownloader downloader,
            IEnhancementProvider enhancementProvider
            )
        {
            var downloaders = new[] {downloader};
            A.CallTo(() => downloader.EnhancementRequests).Returns(enhancementRequests);
            A.CallTo(() => enhancementProvider.GetEnhancement(A<Type>._, filePath, identity))
                .ReturnsNextFromSequence(enhancements);
            var sut = new EnhancementApplicator(downloaders, enhancementProvider);

            sut.ApplyEnhancements(filePath, identity);

            Assert.That(identity.Enhancements, Is.EquivalentTo(enhancements));
        }

        [Test, AutoFakeData]
        public void ApplyEnhancements_DuplicateEnhancementRequestsMade_OnlyUniqueResultsAddedToIdentity(
            string filePath,
            TvReleaseIdentity identity,
            TvDbEnhancement tvDbEnhancement,
            SubDbFileHashEnhancement subDbFileHashEnhancement,
            IEpisodeSubtitleDownloader downloader,
            IEnhancementProvider enhancementProvider
            )
        {
            var expectedEnhancements = new IEnhancement[] {tvDbEnhancement, subDbFileHashEnhancement};
            var downloaders = new[] {downloader};
            var enhancementRequests = new IEnhancementRequest[]
            {
                new EnhancementRequest<TvDbEnhancement>(),
                new EnhancementRequest<TvDbEnhancement>(),
                new EnhancementRequest<SubDbFileHashEnhancement>()
            };
            A.CallTo(() => downloader.EnhancementRequests).Returns(enhancementRequests);
            A.CallTo(
                () =>
                    enhancementProvider.GetEnhancement(A<Type>.That.IsEqualTo(typeof (TvDbEnhancement)), filePath,
                        identity))
                .Returns(tvDbEnhancement);
            A.CallTo(
                () =>
                    enhancementProvider.GetEnhancement(A<Type>.That.IsEqualTo(typeof (SubDbFileHashEnhancement)),
                        filePath, identity))
                .Returns(subDbFileHashEnhancement);
            var sut = new EnhancementApplicator(downloaders, enhancementProvider);

            sut.ApplyEnhancements(filePath, identity);

            Assert.That(identity.Enhancements, Is.EquivalentTo(expectedEnhancements));
        }

        [Test, AutoFakeData]
        public void ApplyEnhancements_NullEnhancementsReturned_OnlyNonNullResultsAddedToIdentity(string filePath,
            TvReleaseIdentity identity,
            IEnumerable<IEnhancementRequest> enhancementRequests,
            IEnhancement[] enhancements,
            IEpisodeSubtitleDownloader downloader,
            IEnhancementProvider enhancementProvider
            )
        {
            enhancements[0] = null;
            var downloaders = new[] {downloader};
            A.CallTo(() => downloader.EnhancementRequests).Returns(enhancementRequests);
            A.CallTo(() => enhancementProvider.GetEnhancement(A<Type>._, filePath, identity))
                .ReturnsNextFromSequence(enhancements);
            var sut = new EnhancementApplicator(downloaders, enhancementProvider);

            sut.ApplyEnhancements(filePath, identity);

            Assert.That(identity.Enhancements, Is.EquivalentTo(enhancements.Skip(1)));
        }

        [Test, AutoFakeData]
        public void ApplyEnhancements_MultipleDownloaders_AllDownloadersRequestsAreUsed(
            string filePath,
            TvReleaseIdentity identity,
            IEnumerable<IEnhancementRequest> enhancementRequests,
            [Frozen]IEnumerable<IEpisodeSubtitleDownloader> downloaders,
            [Frozen]IEnhancementProvider enhancementProvider,
            EnhancementApplicator sut
            )
        {
            var expectedNumberOfCalls = downloaders.Count() * enhancementRequests.Count();
            foreach (var downloader in downloaders)
            {
                A.CallTo(() => downloader.EnhancementRequests).ReturnsNextFromSequence(enhancementRequests);
            }
            sut.ApplyEnhancements(filePath, identity);

            A.CallTo(() => enhancementProvider.GetEnhancement(A<Type>._, A<string>._, A<TvReleaseIdentity>._)).MustHaveHappened(Repeated.Exactly.Times(expectedNumberOfCalls));
        }
    }
}
