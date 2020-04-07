using System;
using System.Linq;

namespace FileGenerator.Generation
{
    ///<inheritdoc/>
    internal sealed class RandomStringGenerator : IRandomStringGenerator
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
        
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public RandomStringGenerator(IRandomNumberGenerator randomNumberGenerator)
        {
            _randomNumberGenerator = randomNumberGenerator;
        }
        
        ///<inheritdoc/>
        public string Generate(long size)
        {
            if (size < 0)
            {
                throw new ArgumentException("'size' could not be less than zero");
            }

            var data = _randomNumberGenerator.GenerateNextBytes(size);
            return new string(data.Select(x => Chars[x % Chars.Length]).ToArray());
        }
    }
}