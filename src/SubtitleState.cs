using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace SubtitleFetcher
{
	[Serializable]
	public class SubtitleState
	{
		private readonly Dictionary<string, SubtitleStateEntry> dict;

		[XmlIgnore]
		public Dictionary<string, SubtitleStateEntry> Dict
		{
			get { return dict; }
		}
		public List<SubtitleStateEntry> Entries { get; set; }

		public SubtitleState()
		{
			Entries = new List<SubtitleStateEntry>();
			dict = new Dictionary<string, SubtitleStateEntry>();
		}

		public void AddEntry(string file, DateTime timestamp)
		{
			if (!dict.ContainsKey(file))
			{
			    dict.Add(file, new SubtitleStateEntry(file, timestamp));
			}
		}

		public void AddEntries(IEnumerable<string> files, DateTime timestamp)
		{
			foreach (string file in files)
			{
			    if (!dict.ContainsKey(file))
			    {
			        dict.Add(file, new SubtitleStateEntry(file, timestamp));
			    }
			}
		}

		public void Cleanup(int days = 7)
		{
			var removeKeys = new List<string>();
			foreach (SubtitleStateEntry entry in dict.Values)
			{
				if (entry.Timestamp.AddDays(days) <= DateTime.Now)
				{
					string fileName = Path.GetFileNameWithoutExtension(entry.File) + ".nosrt";
					StreamWriter w = File.CreateText(fileName);
					w.Write("No subtitle available");
					w.Flush();
					w.Close();

					removeKeys.Add(entry.File);
				}
			}
			foreach (string key in removeKeys)
				dict.Remove(key);
		}

		public void PostDeserialize()
		{
			foreach (var entry in Entries)
				dict.Add(entry.File, entry);
		}

		public void PreSerialize()
		{
			Entries.Clear();
			Entries.AddRange(dict.Values);
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
