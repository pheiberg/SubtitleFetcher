using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SubtitleFetcher
{
    public class StateSerializer
    {
        private readonly string stateFileName;
        private readonly Logger logger;
        private readonly int giveUpDays;

        public StateSerializer(string stateFileName, Logger logger, int giveUpDays)
        {
            this.stateFileName = stateFileName;
            this.logger = logger;
            this.giveUpDays = giveUpDays;
        }

        public SubtitleState LoadState(IEnumerable<string> files)
        {
            SubtitleState state = null;
            var xs = new XmlSerializer(typeof(SubtitleState));
            try
            {
                logger.Log("Loading state from {0}...", stateFileName);
                using (TextReader reader = new StreamReader(stateFileName))
                {
                    state = (SubtitleState) xs.Deserialize(reader);
                    if (state != null)
                    {
                        state.PostDeserialize();
                        logger.Log("State loaded. {0} entries...", state.Dict.Count);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Log("Could not load state. Exception: {0}.", e.Message);
            }
            if (state == null)
                state = new SubtitleState();
            state.Cleanup(giveUpDays);
            InitSubtitleState(files, state);
            return state;
        }

        public void SaveState(SubtitleState state)
        {
            var xs = new XmlSerializer(typeof(SubtitleState));
            try
            {
                using (TextWriter writer = new StreamWriter(stateFileName))
                {
                    logger.Log("Saving state to {0}...", stateFileName);
                    state.PreSerialize();
                    xs.Serialize(writer, state);
                    logger.Log("State saved.");
                }
            }
            catch (Exception e)
            {
                logger.Log("Could not save state. Exception: {0}.", e.Message);
            }
        }

        private void InitSubtitleState(IEnumerable<string> files, SubtitleState state)
        {
            var inPaths = files.ToList();
            var paths = inPaths.Any() ? inPaths : new List<string> { "." };

            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    logger.Log("Processing directory {0}...", path);
                    state.AddEntries(Directory.GetFiles(path), DateTime.Now);
                }
                else if (File.Exists(path))
                {
                    state.AddEntry(path, DateTime.Now);
                }
            }
        }
    }
}