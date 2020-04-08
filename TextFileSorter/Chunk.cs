namespace TextFileSorter
{
    public class Chunk
    {
        public const int ChunkLength = 1024 * 1024 * 50;
        public const int ChunkSize = 1024 * 1024 * 100;
        
        public Chunk(string chunk)
        {
            Data = chunk;
        }
        
        public string Data { get; }
    }

    public class ReadChunkResult
    {
        public ReadChunkResult(Chunk chunk, bool isLastChunk)
        {
            Chunk = chunk;
            IsLastChunk = isLastChunk;
        }

        public Chunk Chunk { get; }

        public bool IsLastChunk { get; }
    }
}