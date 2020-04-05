using System.Text;

namespace FileGenerator.IO
{
    public interface IEncodingProvider
    {
        Encoding Encoding { get; }
    }
}