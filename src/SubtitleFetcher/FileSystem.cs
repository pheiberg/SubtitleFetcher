using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SubtitleFetcher
{
    public class FileSystem : IFileSystem
    {
        private readonly IEnumerable<string> acceptedExtensions;
        private readonly ILogger logger;

        public FileSystem(IEnumerable<string> acceptedExtensions, ILogger logger)
        {
            this.acceptedExtensions = acceptedExtensions;
            this.logger = logger;
        }

        public IEnumerable<string> GetFilesToProcess(IEnumerable<string> fileLocations)
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
                        .Where(f => IsFileOfAcceptableType(f) && !HasDownloadedSubtitle(f));
                    files.AddRange(validFiles);
                }
                else if (File.Exists(path) && IsFileOfAcceptableType(path))
                {
                    files.Add(path);
                }
            }
            return files;
        }

        public bool HasDownloadedSubtitle(string filePath)
        {
            return File.Exists(CreateSubtitleFileName(filePath)) || File.Exists(CreateSubtitleFileName(filePath, ".nosrt"));
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
                yield break;

            if(!File.Exists(ignoreFileName))
            {
                logger.Error("The specified ignore shows file can't be found.");
                yield break;
            }

            using (TextReader reader = new StreamReader(ignoreFileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line.Trim();
                }
            }
        }

        public void RenameSubtitleFile(string targetSubtitleFile, string sourceFileName)
        {
            File.Delete(targetSubtitleFile);
            File.Move(sourceFileName, targetSubtitleFile);
        }
    }
}