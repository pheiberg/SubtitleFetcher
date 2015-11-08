using System;
using System.Linq;
using Castle.Core.Internal;
using NUnit.Framework;
using SubtitleFetcher.Common.Infrastructure;

namespace UnitTests.SubtitleFetcher.Common.Infrastructure
{
    [TestFixture]
    public class SubtitleStateTests
    {
        [Test, AutoFakeData]
        public void Cleanup_EntriesAreOld_RemoveAllEntriesOlderThanGiveupdays(
            int giveupdays,
            Action<SubtitleStateEntry> removalAction,
            SubtitleStateEntry[] entries,
            SubtitleState state)
        {
            var toBeRemoved = entries.Take(2);
            toBeRemoved.ForEach(e => e.Timestamp = DateTime.Now.AddDays(-giveupdays - 1));
            var toKeep = entries.Skip(2);
            toKeep.ForEach(e => e.Timestamp = DateTime.Now);
            state.Entries.Clear();
            state.Entries.AddRange(entries);

            state.Cleanup(giveupdays, removalAction);

            Assert.That(state.Entries.Select(s => s.File), Is.EquivalentTo(toKeep.Select(s => s.File)));
        }

        [Test, AutoFakeData]
        public void Update_ExistingEntriesMatching_LeaveExistingItemsUnchanged(
            SubtitleStateEntry[] entries,
            SubtitleState state
            )
        {
            var failedFiles = entries.Select(e => e.File);
            state.Entries.Clear();
            state.Entries.AddRange(entries);

            state.Update(failedFiles);

            Assert.That(entries.Select(e => new { e.File, e.Timestamp }), 
                Is.EquivalentTo(state.Entries.Select(e => new { e.File, e.Timestamp })));
        }

        [Test, AutoFakeData]
        public void Update_NewEntries_AddNewEntries(
            SubtitleStateEntry[] newEntries,
            SubtitleStateEntry[] existingEntries,
            SubtitleState state)
        {
            var failedFiles = existingEntries.Concat(newEntries).Select(e => e.File);
            state.Entries.Clear();
            state.Entries.AddRange(existingEntries);

            state.Update(failedFiles);

            Assert.That(state.Entries.Select(e => e.File), Is.EquivalentTo(failedFiles));
        }

        [Test, AutoFakeData]
        public void Update_RemovedEntries_RemoveMissingItems(
            SubtitleStateEntry[] entries,
            SubtitleState state)
        {
            var failedFiles = entries.Skip(1).Select(e => e.File);
            state.Entries.Clear();
            state.Entries.AddRange(entries);

            state.Update(failedFiles);

            Assert.That(state.Entries.Select(e => e.File), Is.EquivalentTo(failedFiles));
        }
    }
}
