namespace FileGenerator.Generation
{
    /// <summary>
    /// Generate chunk (piece of data to be written to file)
    /// </summary>
    public interface IChunkGenerator
    {
        /// <summary>
        /// Generate next random chunk of particular size
        /// </summary>
        /// <param name="chunkSize">Size of chunk</param>
        /// <returns>Generated chunk</returns>
        string GenerateNext(long chunkSize);
    }
}