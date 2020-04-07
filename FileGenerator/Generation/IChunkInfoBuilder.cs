using FileGenerator.Domain;

namespace FileGenerator.Generation
{
    /// <summary>
    /// Builder responsible for generation information about chunk entries
    /// </summary>
    public interface IChunkInfoBuilder
    {
        /// <summary>
        /// Build information about chunk entries
        /// </summary>
        /// <param name="chunkSize">Size of chunk in bytes</param>
        /// <returns>Information about chunk entries</returns>
        ChunkInfo Build(long chunkSize);
    }
}