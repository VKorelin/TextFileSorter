using System;

namespace FileGenerator.Generation
{
    internal sealed class EntryGenerator : IEntryGenerator
    {
        private const int MaxNumberSize = 7;
        
        private readonly IRandomStringGenerator _randomStringGenerator;
        private readonly IRandomNumberGenerator _numberGenerator;

        public EntryGenerator(IRandomStringGenerator randomStringGenerator, IRandomNumberGenerator numberGenerator)
        {
            _randomStringGenerator = randomStringGenerator;
            _numberGenerator = numberGenerator;
        }

        public string Generate(int size)
        {
            if (size < 6)
            {
                throw new ArgumentException("Each entry line length should have at least 6 chars");
            }
            
            //Minis ' ', '.', '\n', '\r' characters
            size -= 4;

            //Max number size that can be generated is MaxNumberSize. If entry line is short take only half of size for number (e.g. '4. a')
            var numberSize = Math.Min(size / 2, MaxNumberSize);
            var number = _numberGenerator.Generate(0, (int) Math.Pow(10, numberSize)).ToString();

            var line = _randomStringGenerator.Generate(size - number.Length);
            return $"{number}. {line}\r\n";
        }
    }
}