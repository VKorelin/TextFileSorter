namespace FileGenerator
{
    /// <summary>
    /// Result of file generation
    /// </summary>
    public enum GenerationResult
    {
        /// <summary>
        /// Arguments passed to program are invalid
        /// </summary>
        ArgumentsInvalid,
        
        /// <summary>
        /// Some error occured during file generation
        /// </summary>
        GenerationError,
        
        /// <summary>
        /// File generated successfully
        /// </summary>
        Success
    }
}