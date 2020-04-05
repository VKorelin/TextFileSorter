using System.Text;

namespace FileGenerator.IO
{
    internal sealed class Utf8InfoProvider : IEncodingInfoProvider
    {
        public Encoding CurrentEncoding => Encoding.UTF8;
        
        public long GetBytesCount(string str) => Encoding.UTF8.GetByteCount(str);
        
        public long GetBytesCount(long length) => length;

        public long GetStringLength(long bytesCount) => bytesCount;
    }
}