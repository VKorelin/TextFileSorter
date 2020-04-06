using System.Text;

namespace FileGenerator.Configuration
{
    public interface IConfigurationProvider
    {
        Encoding Encoding { get; }
    }
}