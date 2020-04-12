using System.Text;

namespace TextFileSorter.Configuration
{
    public interface IConfigurationProvider
    {
        Encoding Encoding { get; }

        string OutputFolder { get; }

        /// <summary>
        /// Set RAM limit in bytes
        /// </summary>
        long RamLimit { get; }

        /// <summary>
        /// Max number of thread in the app
        /// </summary>
        int ThreadCount { get; }
        
        /// <summary>
        /// Max number of chunks for one thread during merge operation
        /// </summary>
        int MexChunksNumberInMerge { get; }
    }
}