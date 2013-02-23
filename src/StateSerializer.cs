using System;
using System.IO;
using System.Xml.Serialization;

namespace SubtitleFetcher
{
    public class StateSerializer
    {
        private readonly string stateFileName;
        private readonly ILogger logger;

        public StateSerializer(string stateFileName, ILogger logger)
        {
            this.stateFileName = stateFileName;
            this.logger = logger;
        }

        public SubtitleState LoadState()
        {
            if (!File.Exists(stateFileName))
            {
                logger.Log("No previous state can be found at {0}.", stateFileName);
                return new SubtitleState();
            }

            SubtitleState state = null;
            var xs = new XmlSerializer(typeof(SubtitleState));
            try
            {
                logger.Log("Loading state from {0}...", stateFileName);
                using (var reader = new StreamReader(stateFileName))
                {
                    state = (SubtitleState) xs.Deserialize(reader);
                    if (state != null)
                    {
                        logger.Log("State loaded. {0} entries.", state.Entries.Count);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Log("Could not load state. Exception: {0}.", e.Message);
            }
            return state ??  new SubtitleState();
        }

        public void SaveState(SubtitleState state)
        {
            var xs = new XmlSerializer(typeof(SubtitleState));
            try
            {
                using (var writer = new StreamWriter(stateFileName))
                {
                    logger.Log("Saving state to {0}...", stateFileName);
                    xs.Serialize(writer, state);
                    logger.Log("State saved.");
                }
            }
            catch (Exception e)
            {
                logger.Log("Could not save state. Exception: {0}.", e.Message);
            }
        }
    }
}