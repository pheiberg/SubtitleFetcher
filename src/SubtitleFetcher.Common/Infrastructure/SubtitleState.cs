using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitleFetcher.Common.Infrastructure
{
	[Serializable]
	public class SubtitleState
	{
		public List<SubtitleStateEntry> Entries { get; set; }

		public SubtitleState()
		{
			Entries = new List<SubtitleStateEntry>();
		}
    
	    public void Cleanup(int days, Action<SubtitleStateEntry> removeAction)
		{
			var toBeRemoved = Entries.Where(entry => entry.Timestamp.AddDays(days) <= DateTime.Now).ToList();
	        foreach (var entry in toBeRemoved)
			{
                removeAction(entry);
			    Entries.Remove(entry);
			}
		}

	    public void Update(IEnumerable<string> failedFiles)
	    {
	        var updatedState = from failed in failedFiles
	                           join existing in Entries
	                               on failed equals existing.File into filesToReprocess
	                           from oldEntry in filesToReprocess.DefaultIfEmpty()
                               select new SubtitleStateEntry(failed, oldEntry == null ? DateTime.Now : oldEntry.Timestamp);

            Entries = new List<SubtitleStateEntry>(updatedState);
        }
	}

	[Serializable]
	public class SubtitleStateEntry
	{
		public string File { get; set; }
		public DateTime Timestamp { get; set; }

		public SubtitleStateEntry()
		{
			File = String.Empty;
			Timestamp = new DateTime();
		}

		public SubtitleStateEntry(string file, DateTime timestamp)
		{
			File = file;
			Timestamp = timestamp;
		}
	}
}
