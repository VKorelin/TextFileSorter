using System;

namespace FileGenerator.Generation
{
    internal sealed class RandomStringGenerator : IRandomStringGenerator
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";

        private readonly Random _random;

        public RandomStringGenerator()
        {
            _random = new Random();
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
                data[i] = Chars[_random.Next(Chars.Length)];
            }

            return new string(data);
        }
    }
}