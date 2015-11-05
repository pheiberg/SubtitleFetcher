using System;
using System.IO;

namespace SubtitleFetcher.Common.Hashing
{
    public class FileHasher
    {
        const int BlockSize = 64 * 1024;
        private readonly IHashCalculator _hashProvider;

        public FileHasher(IHashCalculator hashProvider)
        {
            if (hashProvider == null) throw new ArgumentNullException(nameof(hashProvider));
            _hashProvider = hashProvider;
        }

        public byte[] CreateHash(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            
            var buffer = ReadBeginningAndEnd(stream);
            return _hashProvider.ComputeHash(buffer);            
        }

        private static byte[] ReadBeginningAndEnd(Stream stream)
        {
            using (var binaryReader = new BinaryReader(stream))
            {
                var buffer = new byte[BlockSize * 2];
                binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
                binaryReader.Read(buffer, 0, BlockSize);
                binaryReader.BaseStream.Seek(-BlockSize, SeekOrigin.End);
                binaryReader.Read(buffer, BlockSize, BlockSize);
                return buffer;
            }
        }
    }
}
