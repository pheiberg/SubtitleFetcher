using System.Collections.Generic;
using System.Linq;
using SubtitleFetcher.Common.Infrastructure;
using SubtitleFetcher.Common.Orchestration;
using SubtitleFetcher.Settings;

namespace SubtitleFetcher
{
    public class DownloaderApplication
    {
        private readonly IFileOperations _fileOperations;
        private readonly IStateSerializer _serializer;
        private readonly IFileProcessor _fileProcessor;

        public DownloaderApplication(IFileOperations fileOperations, IStateSerializer serializer, IFileProcessor fileProcessor)
        {
            _fileOperations = fileOperations;
            _serializer = serializer;
            _fileProcessor = fileProcessor;
        }

        public void Run(Options options)
        {
            var ignoredShows = _fileOperations.GetIgnoredShows(options.IgnoreFileName);
            var state = _serializer.LoadState();
            state.Cleanup(options.GiveupDays, _fileOperations.CreateNosrtFile);

            var filesToProcess = _fileOperations.GetFilesToProcess(new [] { options.Directory }, options.Languages);
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