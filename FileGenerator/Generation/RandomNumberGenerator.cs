using System;

namespace FileGenerator.Generation
{
    ///<inheritdoc/>
    internal sealed class RandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly Random _random;

        public RandomNumberGenerator()
        {
            _random = new Random();
        }

        ///<inheritdoc/>
        public int Generate(int min, int max)
            => _random.Next(min, max);

        ///<inheritdoc/>
        public byte[] GenerateNextBytes(long length)
        {
            var bytes = new byte[length];
            _random.NextBytes(bytes);
            return bytes;
        }
    }
}