using System;
using System.Text;
using FileGenerator.Generation;

namespace FileGenerator.IO
{
    internal sealed class EncodingInfoProviderFactory : IEncodingInfoProviderFactory, IEncodingProvider
    {
        public Encoding Encoding => Encoding.UTF8;
        
        public IEncodingInfoProvider Create()
        {
            if (Encoding.Equals(Encoding.Unicode))
                return new UnicodeInfoProvider();
            
            if (Encoding.Equals(Encoding.UTF8))
                return new Utf8InfoProvider();
            
            throw new Exception($"Unsupported encoding type: {Encoding}");
        }
    }
}