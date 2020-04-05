namespace FileGenerator.Generation
{
    public interface IEncodingInfoProvider
    {
        long GetBytesCount(string str);

        long GetBytesCount(long length);

        long GetStringLength(long bytesCount);
    }

    internal sealed class Utf8InfoProvider : IEncodingInfoProvider
    {
        public long GetBytesCount(string str) => str.Length;
        
        public long GetBytesCount(long length) => length;

        public long GetStringLength(long bytesCount) => bytesCount;
    }

    internal sealed class UnicodeInfoProvider : IEncodingInfoProvider
    {
        public long GetBytesCount(string str) => GetBytesCount(str.Length);

        public long GetBytesCount(long length) => 2 * length;

        public long GetStringLength(long bytesCount) => bytesCount / 2;
    }
}