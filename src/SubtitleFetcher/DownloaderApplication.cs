using System.Linq;

namespace SubtitleFetcher
{
    public class DownloaderApplication
    {
        private readonly IFileSystem fileSystem;
        private readonly IStateSerializer serializer;
        private readonly FileProcessorFactory fileProcessorFactory;

        public DownloaderApplication(IFileSystem fileSystem, IStateSerializer serializer, FileProcessorFactory fileProcessorFactory)
        {
            this.fileSystem = fileSystem;
            this.serializer = serializer;
            this.fileProcessorFactory = fileProcessorFactory;
        }

        public void Run(Options options)
        {
            var ignoredShows = fileSystem.GetIgnoredShows(options.IgnoreFileName);
            var state = serializer.LoadState();
            state.Cleanup(options.GiveupDays, fileSystem.CreateNosrtFile);

            var filesToProcess = fileSystem.GetFilesToProcess(options.Files);
            var fileProcessor = fileProcessorFactory.Create(ignoredShows);
            var failedFiles = (filesToProcess.Where(file => !fileProcessor.ProcessFile(file))).ToList();

            state.Update(failedFiles);
            serializer.SaveState(state);
        }
    }
}