using System;

namespace FileGenerator.Generation
{
    internal sealed class RandomStringGenerator : IRandomStringGenerator
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
        
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public RandomStringGenerator(IRandomNumberGenerator randomNumberGenerator)
        {
            _randomNumberGenerator = randomNumberGenerator;
        }
        
        public string Generate(long size)
        {
            if (size < 0)
            {
                throw new ArgumentException("'size' could not be less than zero");
            }

            var data = new char[size];
            for (var i = 0; i < size; i++)
            {
                data[i] = Chars[_randomNumberGenerator.Generate(0, Chars.Length)];
            }

            return new string(data);
        }
    }
}