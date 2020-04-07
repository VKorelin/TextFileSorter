namespace FileGenerator.Generation
{
    /// <summary>
    /// Generates output file
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Generate output file with random lines
        /// </summary>
        /// <param name="fileSize">Size of file in bytes</param>
        void Generate(long fileSize);
    }
}