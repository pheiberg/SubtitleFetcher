using System.Linq;

namespace SubtitleFetcher
{
    public class DownloaderApplication
    {
        private readonly IFileSystem fileSystem;
        private readonly IStateSerializer serializer;
        private readonly IFileProcessor fileProcessor;

        public DownloaderApplication(IFileSystem fileSystem, IStateSerializer serializer, IFileProcessor fileProcessor)
        {
            this.fileSystem = fileSystem;
            this.serializer = serializer;
            this.fileProcessor = fileProcessor;
        }

        public void Run(Options options)
        {
            var ignoredShows = fileSystem.GetIgnoredShows(options.IgnoreFileName);
            var state = serializer.LoadState();
            state.Cleanup(options.GiveupDays, fileSystem.CreateNosrtFile);

            var filesToProcess = fileSystem.GetFilesToProcess(options.Files, options.Languages);
            var failedFiles = (filesToProcess.Where(file => !fileProcessor.ProcessFile(file, ignoredShows))).ToList();

            state.Update(failedFiles);
            serializer.SaveState(state);
        }
    }
}