using System;
using System.Linq;

namespace TextFileSorter
{
    public class Entry : IComparable<Entry>
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
                throw new ArgumentException($"Entry number should be integer but was {entryArr[0]}", nameof(entry));
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
            if (other == null)
            {
                return 1;
            }

            var lineCompareResult = string.CompareOrdinal(Line, other.Line);
            return lineCompareResult == 0 ? Number.CompareTo(other.Number) : lineCompareResult;
        }
        
        public override string ToString()
            => $"{Number}. {Line}";
        
        public string BuildReverseEntry()
            => $"{Line}. {Number}";
    }
}