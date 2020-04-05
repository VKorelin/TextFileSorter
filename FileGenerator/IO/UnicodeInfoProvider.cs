using System.Text;

namespace FileGenerator.IO
{
    internal sealed class UnicodeInfoProvider : IEncodingInfoProvider
    {
        public Encoding CurrentEncoding => Encoding.Unicode;
        
        public long GetBytesCount(string str) => GetBytesCount(str.Length);

        public long GetBytesCount(long length) => 2 * length;

        public long GetStringLength(long bytesCount) => bytesCount / 2;
    }
}