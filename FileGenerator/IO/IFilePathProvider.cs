namespace FileGenerator.IO
{
    /// <summary>
    /// Provide of generated file path and name
    /// </summary>
    public interface IFilePathProvider
    {
        /// <summary>
        /// Get generated file path
        /// </summary>
        string GetPath();
    }
}