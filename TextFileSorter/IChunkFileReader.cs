using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TextFileSorter
{
    public interface IChunkFileReader : IDisposable
    {
        ReadChunkResult ReadNextChunk(int shift);
    }
    
    internal sealed class ChunkFileReader : IChunkFileReader
    {
        private readonly Encoding _encoding;
        private readonly FileStream _reader;
        
        public ChunkFileReader(string fileName, Encoding encoding)
        {
            _encoding = encoding;
            _reader = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }
        
        public ReadChunkResult ReadNextChunk(int shift)
        {
            _reader.Seek(_reader.Position + shift, SeekOrigin.Begin);
            
            var chunk = new byte[Chunk.ChunkSize];
            var bytesRead = _reader.Read(chunk, 0, Chunk.ChunkSize);

            if (_reader.Position >= _reader.Length)
            {
                return new ReadChunkResult(new Chunk(GetUnicodeString(chunk.Take(bytesRead).ToArray())), true);
            }

            return new ReadChunkResult(new Chunk(GetUnicodeString(chunk)), false);
        }

        private string GetUnicodeString(byte[] chunkBytes)
        {
            var chunk = _encoding.GetString(chunkBytes);
            return chunk[0] == (char) 65279 ? chunk.Remove(0, 1) : chunk;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}