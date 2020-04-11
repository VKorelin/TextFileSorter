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

            if (!int.TryParse(entryArr[0], out var number))
            {
                throw new ArgumentException($"Entry number should be integer less but was {entryArr[0]}", nameof(entry));
            }

            var line = entryArr.Length > 2 ? string.Join(string.Empty, entryArr.Skip(1)) : entryArr[1];
            return new Entry(number, line);
        }

        private readonly int _number;
        private readonly string _line;
        
        private Entry(int number, string line)
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