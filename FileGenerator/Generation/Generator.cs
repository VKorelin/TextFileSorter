using System;
using System.IO;
using System.Text;

namespace FileGenerator.Generation
{
    public class Generator : IGenerator
    {
        private const int BufferSize = 1024 * 1024 * 8;
        
        private readonly IRandomStringGenerator _stringGenerator;
        private readonly Encoding _encoding;
        private readonly IStringSizeCalculator _stringSizeCalculator;

        public Generator(IRandomStringGenerator stringGenerator, IEncodingFactory encodingFactory)
        {
            _stringGenerator = stringGenerator;
            _encoding = encodingFactory.Encoding;
            _stringSizeCalculator = encodingFactory.CreateCalculator();
        }
        
        public void Generate(long fileSize)
        {
            long currentFileSize = 0;
            
            using (var stream = new FileStream("data.txt", FileMode.CreateNew))
            {
                using (var streamWriter = new StreamWriter(stream, _encoding))
                {
                    var stringBuilder = new StringBuilder();

                    while (currentFileSize < fileSize)
                    {
                        var currentBufferSize = Math.Min(fileSize - currentFileSize, BufferSize);
                        long sbSize = 0;
                        
                        while (sbSize < currentBufferSize)
                        {
                            stringBuilder.AppendLine(_stringGenerator.Generate(5));
                            sbSize = _stringSizeCalculator.CalculateSize(stringBuilder);
                        }

                        currentFileSize += sbSize;
                        streamWriter.WriteLine(stringBuilder);
                        stringBuilder.Clear();
                    }
                    
                    stream.Flush();
                }
            }
        }
    }
}