using System;
using System.Linq;
using NUnit.Framework;
using SubtitleFetcher.Common.Infrastructure;

namespace UnitTests.SubtitleFetcher.Common.Infrastructure
{
    [TestFixture]
    public class SubtitleStateTests
    {
        [Test]
        public void Cleanup_EntriesAreOld_RemoveAllEntriesOlderThanGiveupdays()
        {
            var state = new SubtitleState();
            state.Entries.AddRange(
                new[]{
                        new SubtitleStateEntry("keep1", DateTime.Now.AddDays(-1)), 
                        new SubtitleStateEntry("remove1", DateTime.Now.AddDays(-8)),
                        new SubtitleStateEntry("remove2", DateTime.Now.AddDays(-10)),
                        new SubtitleStateEntry("keep2", DateTime.Now.AddDays(-6)),
                    }
                );
            
            state.Cleanup(7, x => {});

            Assert.That(state.Entries.Select(s => s.File), Has.All.StringStarting("keep"));
        }

        [Test]
        public void Update_ExistingEntriesMatching_LeaveExistingItemsUnchanged()
        {
            var state = new SubtitleState();
            state.Entries.AddRange(
                new[]{
                        new SubtitleStateEntry("keep1", new DateTime(2012, 1, 1)), 
                        new SubtitleStateEntry("remove", new DateTime(2012, 1, 1)),
                        new SubtitleStateEntry("keep2", new DateTime(2012, 1, 1)),
                    }
                );

            state.Update(new[]{ "keep1", "keep2" });

            Assert.That(state.Entries.All(s => s.File.StartsWith("keep") && s.Timestamp == new DateTime(2012, 1, 1)), Is.True);
            Assert.That(state.Entries.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Update_NewEntries_AddNewEntries()
        {
            var state = new SubtitleState();
            state.Entries.AddRange(
                new[]{
                        new SubtitleStateEntry("keep1", new DateTime(2012, 1, 1)), 
                        new SubtitleStateEntry("remove", new DateTime(2012, 1, 1)),
                        new SubtitleStateEntry("keep2", new DateTime(2012, 1, 1)),
                    }
                );

            state.Update(new[] { "keep1", "keep2", "new" });

            Assert.That(state.Entries.Count(s => s.File.StartsWith("new")), Is.EqualTo(1));
        }

        [Test]
        public void Update_RemovedEntries_RemoveMissingItems()
        {
            var state = new SubtitleState();
            state.Entries.AddRange(
                new[]{
                        new SubtitleStateEntry("keep1", new DateTime(2012, 1, 1)), 
                        new SubtitleStateEntry("remove", new DateTime(2012, 1, 1)),
                        new SubtitleStateEntry("keep2", new DateTime(2012, 1, 1)),
                    }
                );

            state.Update(new[] { "keep1", "keep2" });

            Assert.That(state.Entries.Any(s => s.File.StartsWith("remove")), Is.False);
        }
    }
}
