using System;

namespace FileGenerator.Generation
{
    internal sealed class RandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly Random _random;

        public RandomNumberGenerator()
        {
            _random = new Random();
        }

        public int Generate(int min, int max)
            => _random.Next(min, max);

        public byte[] GenerateNextBytes(long size)
        {
            var bytes = new byte[size];
            _random.NextBytes(bytes);
            return bytes;
        }
    }
}