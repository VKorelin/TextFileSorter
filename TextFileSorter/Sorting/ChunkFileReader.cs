using System.IO;
using System.Linq;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    internal sealed class ChunkFileReader : IChunkFileReader
    {
        private readonly FileStream _reader;
        private readonly int _chunkSize;
        
        public ChunkFileReader(string fileName, IConfigurationProvider configurationProvider)
        {
            _chunkSize = configurationProvider.ChunkSize;
            _reader = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }
        
        public ReadChunkResult ReadNextChunk(int shift)
        {
            _reader.Seek(_reader.Position + shift, SeekOrigin.Begin);
            
            var chunk = new byte[_chunkSize];
            var bytesRead = _reader.Read(chunk, 0, _chunkSize);
            return _reader.Position == _reader.Length 
                ? new ReadChunkResult(chunk.Take(bytesRead).ToArray(), true) 
                : new ReadChunkResult(chunk, false);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}