using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubtitleFetcher.Common;

namespace SubtitleFetcher
{
    public class FileSystem : IFileSystem
    {
        private readonly IEnumerable<string> acceptedExtensions;
        private readonly ILogger logger;

        public FileSystem(FileTypeSettings fileTypeSettings, ILogger logger)
        {
            this.acceptedExtensions = fileTypeSettings.AcceptedExtensions;
            this.logger = logger;
        }

        public IEnumerable<string> GetFilesToProcess(IEnumerable<string> fileLocations, IEnumerable<string> languages)
        {
            var inPaths = fileLocations.ToList();
            var paths = inPaths.Any() ? inPaths : new List<string> { "." };

            var files = new List<string>();
            foreach (string path in paths)
            {
                if (Directory.Exists(path))
                {
                    logger.Verbose("Processing directory {0}...", path);
                    var validFiles = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                        .Where(f => IsFileOfAcceptableType(f) && !HasDownloadedSubtitle(f, languages));
                    files.AddRange(validFiles);
                }
                else if (File.Exists(path) && IsFileOfAcceptableType(path))
                {
                    files.Add(path);
                }
            }
            return files;
        }

        public IEnumerable<string> GetSubtitleNamesIndicatingNoDownloadShouldBeMade(string filePath, IEnumerable<string> languages)
        {
            yield return CreateSubtitleFileName(filePath);
            yield return CreateSubtitleFileName(filePath, ".nosrt");
            var lang = languages.FirstOrDefault();
            if(lang != null)
                yield return CreateSubtitleFileName(filePath, "." + lang + ".srt");
        }

        public bool HasDownloadedSubtitle(string filePath, IEnumerable<string> languages)
        {
            var subNames = GetSubtitleNamesIndicatingNoDownloadShouldBeMade(filePath, languages);
            return subNames.Any(File.Exists);
        }

        private bool IsFileOfAcceptableType(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            return acceptedExtensions.Contains(ext);
        }

        public void CreateNosrtFile(SubtitleStateEntry entry)
        {
            string fileName = CreateSubtitleFileName(entry.File, ".nosrt");
            using (var writer = File.CreateText(fileName))
            {
                writer.Write("No subtitle available");
            }
        }

        public static string CreateSubtitleFileName(string filePath, string extension = ".srt")
        {
            var targetLocation = GetTargetFileNamePrefix(filePath);
            return targetLocation + extension;
        }

        private static string GetTargetFileNamePrefix(string fileName)
        {
            var path = Path.GetDirectoryName(fileName);
            var file = Path.GetFileNameWithoutExtension(fileName);
            var targetLocation = Path.Combine(path, file);
            return targetLocation;
        }

        public IEnumerable<string> GetIgnoredShows(string ignoreFileName)
        {
            if (ignoreFileName == null)
                return Enumerable.Empty<string>();

            if(!File.Exists(ignoreFileName))
            {
                logger.Error("The specified ignore shows file can't be found.");
                return Enumerable.Empty<string>();
            }

            var ignoredShows = new List<string>();
            using (TextReader reader = new StreamReader(ignoreFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    ignoredShows.Add(line.Trim());
                }
            }
            logger.Debug(string.Format("Ignore shows file loaded. {0} shows ignored.", ignoredShows.Count()));
            return ignoredShows;
        }

        public void RenameSubtitleFile(string sourceFileName, string targetSubtitleFile)
        {
            File.Delete(targetSubtitleFile);
            File.Move(sourceFileName, targetSubtitleFile);
        }
    }

    public class FileTypeSettings
    {
        private readonly IEnumerable<string> acceptedExtensions;

        public FileTypeSettings(IEnumerable<string> acceptedExtensions)
        {
            this.acceptedExtensions = acceptedExtensions;
        }

        public IEnumerable<string> AcceptedExtensions
        {
            get { return acceptedExtensions; }
        }
    }
}