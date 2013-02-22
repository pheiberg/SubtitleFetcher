using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		    foreach (string file in files.Where(file => !dict.ContainsKey(file)))
		    {
		        dict.Add(file, new SubtitleStateEntry(file, timestamp));
		    }
		}

	    public void Cleanup(int days)
		{
			var removeKeys = new List<string>();
			foreach (SubtitleStateEntry entry in dict.Values)
			{
				if (entry.Timestamp.AddDays(days) <= DateTime.Now)
				{
				    CreateNosrtFile(entry);
				    removeKeys.Add(entry.File);
				}
			}
			foreach (string key in removeKeys)
			{
			    dict.Remove(key);
			}
		}

	    private static void CreateNosrtFile(SubtitleStateEntry entry)
	    {
	        string fileName = Path.GetFileNameWithoutExtension(entry.File) + ".nosrt";
	        using (var w = File.CreateText(fileName))
	        {
	            w.Write("No subtitle available");
	            w.Flush();
	            w.Close();
	        }
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
