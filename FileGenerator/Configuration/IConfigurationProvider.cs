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
    }
}