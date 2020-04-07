using System.Text;

namespace FileGenerator.IO
{
    public interface IEncodingInfoProvider
    {
        Encoding CurrentEncoding { get; }

        long GetBytesCount(long length);

        long GetStringLength(long bytesCount);
    }
}