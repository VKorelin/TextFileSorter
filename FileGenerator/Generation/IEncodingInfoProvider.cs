namespace FileGenerator.Generation
{
    public interface IEncodingInfoProvider
    {
        long CalculateSize(string stringBuilder);

        long GetStringLength(long bytesCount);
    }

    internal sealed class Utf8InfoProvider : IEncodingInfoProvider
    {
        public long CalculateSize(string stringBuilder) => stringBuilder.Length;

        public long GetStringLength(long bytesCount) => bytesCount;
    }

    internal sealed class UnicodeInfoProvider : IEncodingInfoProvider
    {
        public long CalculateSize(string stringBuilder) => 2 * stringBuilder.Length;

        public long GetStringLength(long bytesCount) => bytesCount / 2;
    }
}