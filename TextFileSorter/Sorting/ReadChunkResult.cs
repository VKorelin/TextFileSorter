namespace TextFileSorter.Sorting
{
    public class ReadChunkResult
    {
        public ReadChunkResult(byte[] chunk, bool isLastChunk)
        {
            Chunk = chunk;
            IsLastChunk = isLastChunk;
        }

        public byte[] Chunk { get; }

        public bool IsLastChunk { get; }
    }
}