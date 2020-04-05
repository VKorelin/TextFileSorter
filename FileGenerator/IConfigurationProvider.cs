using System.Text;

namespace FileGenerator
{
    public interface IConfigurationProvider
    {
        Encoding Encoding { get; }
    }
}