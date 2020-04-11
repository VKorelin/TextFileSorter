namespace FileGenerator.Generation
{
    /// <summary>
    /// Generate chunk (piece of data to be written to file)
    /// </summary>
    public interface IChunkGenerator
    {
        /// <summary>
        /// Generate next chunk
        /// </summary>
        /// <param name="chunkSize">Size of chunk</param>
        string GenerateNext(long chunkSize);
    }
}