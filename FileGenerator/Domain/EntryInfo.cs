using System;

namespace FileGenerator.Domain
{
    public class EntryInfo
    {
        /// <summary>
        /// Max Number is 10000000
        /// </summary>
        public const int MaxNumberLength = 7;

        /// <summary>
        /// Service length: dot, space, \r, \n
        /// </summary>
        public const int ServiceLength = 4;

        /// <summary>
        /// New line in the end of each entry
        /// </summary>
        public const string NewLineEnding = "\r\n";

        /// <summary>
        /// Shortest entry should have at least one digit and one character
        /// </summary>
        public static readonly int MinLength = ServiceLength + 2;

        public EntryInfo(int numberLength, int lineLength, bool isDuplicated = false)
        {
            if (numberLength < 1)
                throw new ArgumentException("NumberLength cannot be less than 1");
            
            if (lineLength < 1)
                throw new ArgumentException("LineLength cannot be less than 1");

            NumberLength = numberLength;
            LineLength = lineLength;
            IsDuplicated = isDuplicated;
        }

        public int NumberLength { get; }
        
        public int LineLength { get; }
        
        public bool IsDuplicated { get; }
    }
}