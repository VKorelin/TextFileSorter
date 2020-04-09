using System.Collections.Generic;

namespace TextFileSorter.Sorting
{
    public class ReadChunkResult
    {
        public ReadChunkResult(IList<string> chunk, bool isLastChunk)
        {
            Chunk = chunk;
            IsLastChunk = isLastChunk;
        }

        public IList<string> Chunk { get; }

        public bool IsLastChunk { get; }
    }
}