using System.Collections.Generic;
using System.IO;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    internal sealed class ChunkFileReader : IChunkFileReader
    {
        private readonly StreamReader _reader;
        private readonly long _chunkLength;
        
        public ChunkFileReader(string fileName, IConfigurationProvider configurationProvider)
        {
            var encoding = configurationProvider.Encoding;
            
            _chunkLength = encoding.GetMaxCharCount((int) configurationProvider.RamLimit / configurationProvider.ThreadCount);
            _reader = new StreamReader(fileName, encoding);
        }
        
        public ReadChunkResult ReadNextChunk()
        {
            var lines = new List<string>();
            long currentLength = 0;
            
            while (currentLength < _chunkLength)
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