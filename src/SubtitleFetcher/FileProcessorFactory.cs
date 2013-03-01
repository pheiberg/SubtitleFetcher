using System.Collections.Generic;
using StructureMap;

namespace SubtitleFetcher
{
    public class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly IContainer container;

        public FileProcessorFactory(IContainer container)
        {
            this.container = container;
        }

        public IFileProcessor Create(IEnumerable<string> ignoredShows)
        {
            return container.With("ignoredShows").EqualTo(ignoredShows).GetInstance<FileProcessor>();
        }
    }
}