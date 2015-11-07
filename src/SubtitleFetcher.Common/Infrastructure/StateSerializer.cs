using System;
using System.IO;
using System.Xml.Serialization;
using SubtitleFetcher.Common.Logging;

namespace SubtitleFetcher.Common.Infrastructure
{
    public class StateSerializer : IStateSerializer
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
                logger.Debug("StateSerializer", string.Format("No previous state can be found at {0}.", stateFileName));
                return new SubtitleState();
            }

            SubtitleState state = null;
            var xs = new XmlSerializer(typeof(SubtitleState));
            try
            {
                logger.Debug("StateSerializer", "Loading state from {0}...", stateFileName);
                using (var reader = new StreamReader(stateFileName))
                {
                    state = (SubtitleState) xs.Deserialize(reader);
                    if (state != null)
                    {
                        logger.Debug("StateSerializer", "State loaded. {0} entries.", state.Entries.Count);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("StateSerializer", "Could not load state. Exception: {0}.", e.Message);
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
                    logger.Debug("StateSerializer", string.Format("Saving state to {0}...", stateFileName));
                    xs.Serialize(writer, state);
                    logger.Debug("StateSerializer", "State saved.");
                }
            }
            catch (Exception e)
            {
                logger.Error("StateSerializer", "Could not save state. Exception: {0}.", e.Message);
            }
        }
    }
}