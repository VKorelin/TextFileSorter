using System.Text;

namespace TextFileSorter.Configuration
{
    internal sealed class UnicodeInfoProvider : IEncodingInfoProvider
    {
        public Encoding Encoding => Encoding.Unicode;
        
        public string GetString(byte[] bytes)
        {
            var chunk = Encoding.GetString(bytes);
            return chunk[0] == (char) 65279 ? chunk.Remove(0, 1) : chunk;
        }
    }
}