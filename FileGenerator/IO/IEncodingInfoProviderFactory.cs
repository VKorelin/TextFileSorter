using FileGenerator.Generation;

namespace FileGenerator.IO
{
    public interface IEncodingInfoProviderFactory
    {
        IEncodingInfoProvider Create();
    }
}