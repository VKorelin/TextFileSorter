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
            if (size < 4)
            {
                throw new ArgumentException("Each entry line length should have at least 4 chars");
            }
            
            //Minis ' ' and '.' characters
            size -= 2;

            //Max number size that can be generated is MaxNumberSize. If entry line is short take only half of size for number (e.g. '4. a')
            var numberSize = Math.Min(size / 2, MaxNumberSize);
            var number = _numberGenerator.Generate((int) Math.Pow(10, numberSize)).ToString();

            var line = _randomStringGenerator.Generate(size - number.Length);
            return $"{number}. {line}";
        }
    }
}