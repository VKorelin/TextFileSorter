using System.Text;

namespace TextFileSorter.Configuration
{
    public interface IEncodingInfoProvider
    {
        Encoding Encoding { get; }

        string GetString(byte[] bytes);
    }
}