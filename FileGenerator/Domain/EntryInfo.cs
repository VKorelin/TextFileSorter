using System;

namespace FileGenerator.Domain
{
    public class EntryInfo
    {
        /// <summary>
        /// Max Length of the Entry
        /// </summary>
        public const int MaxEntryLength = 100;
        
        /// <summary>
        /// Max Number is 10000000
        /// </summary>
        public const int MaxNumberLength = 7;

        /// <summary>
        /// Service length: dot, space, \r, \n
        /// </summary>
        public const int ServiceLength = 4;

        /// <summary>
        /// Shortest entry should have at least one digit and one character
        /// </summary>
        public static readonly int MinLength = ServiceLength + 2;

        /// <summary>
        /// Create information about each line in the file - entry
        /// </summary>
        /// <param name="numberLength">Length of the number part of entry</param>
        /// <param name="lineLength">Length of the string part of entry</param>
        /// <exception cref="ArgumentException">Exception will be thrown if line length or number length is less than 1</exception>
        public EntryInfo(int numberLength, int lineLength)
        {
            if (numberLength < 1)
                throw new ArgumentException("NumberLength cannot be less than 1");
            
            if (lineLength < 1)
                throw new ArgumentException("LineLength cannot be less than 1");

            NumberLength = numberLength;
            LineLength = lineLength;
        }

        /// <summary>
        /// Length of the number part of entry
        /// </summary>
        public int NumberLength { get; }
        
        /// <summary>
        /// Length of the string part of entry
        /// </summary>
        public int LineLength { get; }

        /// <summary>
        /// Build entry
        /// </summary>
        /// <param name="number">Entry number</param>
        /// <param name="line">Entry string</param>
        /// <returns>Entry itself</returns>
        public static string BuildEntry(int number, string line)
            => $"{number}. {line}";
    }
}