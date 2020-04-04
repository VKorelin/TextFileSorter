using System.Text;

namespace FileGenerator.Generation
{
    public interface IStringSizeCalculator
    {
        long CalculateSize(StringBuilder stringBuilder);
    }

    internal sealed class Utf8SizeCalculator : IStringSizeCalculator
    {
        public long CalculateSize(StringBuilder stringBuilder)
            => stringBuilder.Length;
    }

    internal sealed class UnicodeSizeCalculator : IStringSizeCalculator
    {
        public long CalculateSize(StringBuilder stringBuilder)
            => 2 * stringBuilder.Length;
    }
}