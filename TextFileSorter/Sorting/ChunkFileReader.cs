using System.Collections.Generic;
using System.IO;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    internal sealed class ChunkFileReader : IChunkFileReader
    {
        private readonly StreamReader _reader;
        private readonly long _chunkSize;
        
        public ChunkFileReader(string fileName, IEncodingInfoProvider encodingInfoProvider)
        {
            _chunkSize = encodingInfoProvider.BufferLength;
            _reader = new StreamReader(fileName, encodingInfoProvider.Encoding);
        }
        
        public ReadChunkResult ReadNextChunk()
        {
            var lines = new List<string>();
            long currentLength = 0;
            
            while (currentLength < _chunkSize)
            {
                var line = _reader.ReadLine();

                if (line == null)
                {
                    return new ReadChunkResult(lines, true);
                }

                currentLength += line.Length;
                lines.Add(line);
            }
            
            return new ReadChunkResult(lines, false);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}