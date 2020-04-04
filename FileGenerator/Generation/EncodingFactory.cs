using System;
using System.Text;

namespace FileGenerator.Generation
{
    internal sealed class EncodingFactory : IEncodingFactory
    {
        public Encoding Encoding => Encoding.Unicode;
        
        public IStringSizeCalculator CreateCalculator()
        {
            if (Encoding.Equals(Encoding.Unicode))
                return new UnicodeSizeCalculator();
            else if (Encoding.Equals(Encoding.Unicode))
                return new Utf8SizeCalculator();
            else
                throw new Exception($"Unsupported encoding type: {Encoding}");
        }
    }
}