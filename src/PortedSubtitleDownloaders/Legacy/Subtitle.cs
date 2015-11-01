using System;

namespace PortedSubtitleDownloaders.Legacy
{
    public class Subtitle
    {
        public string Id { get; }

        public string ProgramName { get; private set; }

        public string FileName { get; private set; }

        public string LanguageCode { get; private set; }

        public Subtitle(string id, string programName, string fileName, string languageCode)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("ID cannot be null or empty!");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty!");
            if (string.IsNullOrEmpty(languageCode))
                throw new ArgumentException("Language code cannot be null or empty!");
            if (languageCode != null && languageCode.Length != 3)
                throw new ArgumentException("Language code must be ISO 639-2 Code!");
            if (!Languages.IsSupportedLanguageCode(languageCode))
                throw new ArgumentException("Language code '" + languageCode + "' is not supported!");
            Id = id;
            ProgramName = programName;
            FileName = fileName;
            LanguageCode = languageCode;
        }
    }
}