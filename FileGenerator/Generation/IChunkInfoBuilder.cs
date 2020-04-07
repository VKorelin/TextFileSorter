using FileGenerator.Domain;

namespace FileGenerator.Generation
{
    public interface IChunkInfoBuilder
    {
        ChunkInfo Build(long bufferSize);
    }
}