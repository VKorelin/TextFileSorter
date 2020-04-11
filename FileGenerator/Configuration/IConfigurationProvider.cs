using System.Text;

namespace FileGenerator.Configuration
{
    /// <summary>
    /// Provides information from appsettings.json
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Encoding that should be used in generated file
        /// </summary>
        Encoding Encoding { get; }
        
        /// <summary>
        /// Generated file name
        /// </summary>
        string OutputFolder { get; }

        /// <summary>
        /// Default size of buffer for generated chunk
        /// </summary>
        int DefaultBufferSize { get; }
    }
}