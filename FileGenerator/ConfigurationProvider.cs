using System.Text;

namespace FileGenerator
{
    internal sealed class ConfigurationProvider : IConfigurationProvider
    {
        public Encoding Encoding => Encoding.Unicode;
    }
}