using System.Text;

namespace TextFileSorter.Configuration
{
    public interface IConfigurationProvider
    {
        int ChunkSize { get; }
        
        int ThreadCount { get; }
        
        Encoding Encoding { get; }
        
        string OutputFolder { get; }
    }
}