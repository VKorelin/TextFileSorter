namespace FileGenerator.Generation
{
    /// <summary>
    /// Generator of random string
    /// </summary>
    public interface IRandomStringGenerator
    {
        /// <summary>
        /// Generate random string
        /// </summary>
        /// <param name="size">Size of string</param>
        /// <returns>Generate string</returns>
        string Generate(long size);
    }
}