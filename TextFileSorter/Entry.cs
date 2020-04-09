using System;
using System.Linq;

namespace TextFileSorter
{
    /// <summary>
    /// Represents line in the file
    /// </summary>
    public struct Entry : IComparable<Entry>
    {
        public static Entry Build(string entry)
        {
            var entryArr = entry.Split(". ");

            if (entryArr.Length < 2)
            {
                throw new ArgumentException($"Entry line has invalid format: {entry}", nameof(entry));
            }

            if (!int.TryParse(entryArr[0], out var number) && number < 1000000000)
            {
                throw new ArgumentException($"Entry number should be integer less than 1000000000 but was {entryArr[0]}", nameof(entry));
            }

            var line = entryArr.Length > 2 ? string.Join(string.Empty, entryArr.Skip(1)) : entryArr[1];
            if (line.Any(x => x == '.'))
            {
                throw new ArgumentException("Line should not contain any dots ('.')");
            }
            
            return new Entry(number, entryArr.Length > 2 ? string.Join(string.Empty, entryArr.Skip(1)) : entryArr[1]);
        }
        
        private Entry(int number, string line)
        {
            Number = number;
            Line = line;
        }

        public int Number { get; }

        public string Line { get; }

        public int CompareTo(Entry other)
        {
            var lineCompareResult = string.CompareOrdinal(Line, other.Line);
            return lineCompareResult == 0 ? Number.CompareTo(other.Number) : lineCompareResult;
        }
        
        public override string ToString()
            => $"{Number}. {Line}";
        
        /// <summary>
        /// To sortable string by line, then by number.
        /// </summary>
        /// <returns>Reversed line</returns>
        public string ToStringReversed()
        {
            var numberStr = Number.ToString();
            return $"{Line}.{numberStr.Length}.{numberStr}";
        }
    }
}