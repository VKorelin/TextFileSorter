using System.Text;

namespace TextFileSorter.Configuration
{
    internal sealed class UnicodeInfoProvider : IEncodingInfoProvider
    {
        private readonly IConfigurationProvider _configurationProvider;

        public UnicodeInfoProvider(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }
        
        public Encoding Encoding => Encoding.Unicode;

        public long BufferLength => _configurationProvider.ChunkSize / 2;
    }
}