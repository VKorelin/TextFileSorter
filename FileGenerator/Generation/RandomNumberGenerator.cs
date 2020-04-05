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

        public int Generate(int max)
            => _random.Next(max);
    }
}