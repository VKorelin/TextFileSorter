using System.Text;

namespace FileGenerator.IO
{
    internal sealed class UnicodeInfoProvider : IEncodingInfoProvider
    {
        public Encoding CurrentEncoding => Encoding.Unicode;
        
        public long GetBytesCount(string str) => Encoding.Unicode.GetByteCount(str);

        public long GetBytesCount(long length) => length * 2;

        public long GetStringLength(long bytesCount) => bytesCount / 2;
    }
}