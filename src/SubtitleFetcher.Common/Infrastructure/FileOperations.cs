using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using SubtitleFetcher.Common.Languages;
using SubtitleFetcher.Common.Logging;

namespace SubtitleFetcher.Common.Infrastructure
{
    public class FileOperations : IFileOperations
    {
        private readonly IEnumerable<string> _acceptedExtensions;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public FileOperations(FileTypeSettings fileTypeSettings, ILogger logger, IFileSystem fileSystem)
        {
            _acceptedExtensions = fileTypeSettings.AcceptedExtensions;
            _logger = logger;
            _fileSystem = fileSystem;
        }

        public IEnumerable<string> GetFilesToProcess(IEnumerable<string> fileLocations, 
            IEnumerable<string> languages)
        {
            var inPaths = fileLocations.ToArray();
            var paths = inPaths.Any() ? inPaths : new[] { "." };

            var files = new List<string>();
            foreach (string path in paths)
            {
                if (_fileSystem.Directory.Exists(path))
                {
                    _logger.Verbose("FileSystem", "Processing directory {0}...", path);
                    var validFiles = _fileSystem.Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                        .Where(f => IsFileOfAcceptableType(f) && !HasDownloadedSubtitle(f, languages));
                    files.AddRange(validFiles);
                }
                else if (_fileSystem.File.Exists(path) && IsFileOfAcceptableType(path))
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

        public IEnumerable<Language> GetDowloadedSubtitleLanguages(string filePath, IEnumerable<Language> languages)
        {
            var filesToCheck = languages.ToDictionary(
                language => language, 
                language => CreateSubtitleFileName(filePath, $".{language.TwoLetterIsoName}.srt"));
            return filesToCheck.Where(item => _fileSystem.File.Exists(item.Value)).Select(item => item.Key);
        }

        public bool HasDownloadedSubtitle(string filePath, IEnumerable<string> languages)
        {
            var subNames = GetSubtitleNamesIndicatingNoDownloadShouldBeMade(filePath, languages);
            return subNames.Any(_fileSystem.File.Exists);
        }

        private bool IsFileOfAcceptableType(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            return _acceptedExtensions.Contains(ext);
        }

        public void CreateNosrtFile(SubtitleStateEntry entry)
        {
            string fileName = CreateSubtitleFileName(entry.File, ".nosrt");
            using (var writer = _fileSystem.File.CreateText(fileName))
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

            if(!_fileSystem.File.Exists(ignoreFileName))
            {
                _logger.Error("Options", "The specified ignore shows file can't be found.");
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
            _logger.Debug("FileSystem", $"Ignore shows file loaded. {ignoredShows.Count} shows ignored.");
            return ignoredShows;
        }

        public void RenameSubtitleFile(string sourceFileName, string targetSubtitleFile)
        {
            _fileSystem.File.Delete(targetSubtitleFile);
            _fileSystem.File.Move(sourceFileName, targetSubtitleFile);
        }
    }
}