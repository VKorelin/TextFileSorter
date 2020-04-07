namespace FileGenerator.Validation
{
    /// <summary>
    /// Command line arguments validator
    /// </summary>
    public interface IArgumentsValidator
    {
        /// <summary>
        /// Validates file size that should be generated
        /// </summary>
        /// <param name="args">Arguments of command line</param>
        /// <param name="fileSize">Size of file</param>
        /// <returns>True if file size is valid</returns>
        bool IsValid(string[] args, out long fileSize);
    }
}