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

            if (!long.TryParse(entryArr[0], out var number))
            {
                throw new ArgumentException($"Entry number should be Int64 but was {entryArr[0]}", nameof(entry));
            }

            var line = entryArr.Length > 2 ? string.Join(string.Empty, entryArr.Skip(1)) : entryArr[1];
            return new Entry(number, line);
        }

        private readonly long _number;
        private readonly string _line;
        
        private Entry(long number, string line)
        {
            _number = number;
            _line = line;
        }

        public int CompareTo(Entry other)
        {
            var lineCompareResult = string.CompareOrdinal(_line, other._line);
            return lineCompareResult == 0 ? _number.CompareTo(other._number) : lineCompareResult;
        }
        
        public override string ToString()
            => _number + ". " + _line;
    }
}