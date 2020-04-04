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
            
            if (Encoding.Equals(Encoding.UTF8))
                return new Utf8SizeCalculator();
            
            throw new Exception($"Unsupported encoding type: {Encoding}");
        }
    }
}