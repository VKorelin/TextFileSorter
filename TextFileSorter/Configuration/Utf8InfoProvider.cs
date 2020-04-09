using System.Text;

namespace TextFileSorter.Configuration
{
    internal sealed class Utf8InfoProvider : IEncodingInfoProvider
    {
        public Encoding Encoding => Encoding.UTF8;

        public string GetString(byte[] bytes)
            => Encoding.GetString(bytes);
    }
}