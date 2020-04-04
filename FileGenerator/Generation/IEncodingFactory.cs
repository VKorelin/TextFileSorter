using System.Text;

namespace FileGenerator.Generation
{
    public interface IEncodingFactory
    {
        Encoding Encoding { get; }

        IStringSizeCalculator CreateCalculator();
    }
}