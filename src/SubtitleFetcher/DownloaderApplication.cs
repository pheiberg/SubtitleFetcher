using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Orchestration;

namespace SubtitleFetcher
{
    public class DownloaderApplication
    {
        private readonly IFileSystem _fileSystem;
        private readonly IStateSerializer _serializer;
        private readonly IFileProcessor _fileProcessor;

        public DownloaderApplication(IFileSystem fileSystem, IStateSerializer serializer, IFileProcessor fileProcessor)
        {
            _fileSystem = fileSystem;
            _serializer = serializer;
            _fileProcessor = fileProcessor;
        }

        public void Run(Options options)
        {
            var ignoredShows = _fileSystem.GetIgnoredShows(options.IgnoreFileName);
            var state = _serializer.LoadState();
            state.Cleanup(options.GiveupDays, _fileSystem.CreateNosrtFile);

            var filesToProcess = _fileSystem.GetFilesToProcess(options.Files, options.Languages);
            var failedFiles = ProcessFiles(filesToProcess, ignoredShows);

            state.Update(failedFiles);
            _serializer.SaveState(state);
        }

        private IEnumerable<string> ProcessFiles(IEnumerable<string> filesToProcess, IEnumerable<string> ignoredShows)
        {
            var ignoredShowsArray = ignoredShows as string[] ?? ignoredShows.ToArray();
            foreach (var file in filesToProcess)
            {
                bool success =_fileProcessor.ProcessFile(file, ignoredShowsArray);
                if (!success)
                {
                    yield return file;
                }
            }
        }
    }
}