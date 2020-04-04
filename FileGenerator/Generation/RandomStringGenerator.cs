using System;
using System.Linq;

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
        
        public string Generate(int size)
        {
            if (size < 0)
            {
                throw new ArgumentException("'size' could not be less than zero");
            }
            
            return new string(Enumerable.Repeat(Chars, size).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}