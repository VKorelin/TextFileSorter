using System.Text;

namespace FileGenerator.IO
{
    internal sealed class Utf8InfoProvider : IEncodingInfoProvider
    {
        public Encoding CurrentEncoding => Encoding.UTF8;
        
        public long GetBytesCount(string str) => str.Length;
        
        public long GetBytesCount(long length) => length;

        public long GetStringLength(long bytesCount) => bytesCount;
    }
}