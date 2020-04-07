namespace FileGenerator
{
    internal interface IBootstrapper
    {
        /// <summary>
        /// Validates arguments and starts generation
        /// </summary>
        /// <param name="args">Program arguments</param>
        /// <returns>Result of file generation</returns>
        GenerationResult Start(string[] args);
    }
}