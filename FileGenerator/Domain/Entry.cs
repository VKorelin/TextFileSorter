namespace FileGenerator.Domain
{
    public class Entry
    {
        public Entry(int number, string line, EntryInfo info)
        {
            Number = number;
            Line = line;
            Info = info;
        }
        
        public int Number { get; }
        
        public string Line { get; }
        
        public EntryInfo Info { get; }

        public override string ToString() => $"{Number}. {Line}";
    }
}