using System.Text;

namespace TextFileSorter.Configuration
{
    internal sealed class Utf8InfoProvider : IEncodingInfoProvider
    {
        private readonly IConfigurationProvider _configurationProvider;

        public Utf8InfoProvider(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }
        
        public Encoding Encoding => Encoding.UTF8;

        public long BufferLength => _configurationProvider.ChunkSize;
    }
}