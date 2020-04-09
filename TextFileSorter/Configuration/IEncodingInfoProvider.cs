using System.Text;

namespace TextFileSorter.Configuration
{
    public interface IEncodingInfoProvider
    {
        Encoding Encoding { get; }

        long BufferLength { get; }
    }
}