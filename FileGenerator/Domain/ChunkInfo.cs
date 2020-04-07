using System.Collections.Generic;

namespace FileGenerator.Domain
{
    public class ChunkInfo
    {
        public ChunkInfo(List<EntryInfo> entryInfos, EntryInfo repeatedEntry)
        {
            EntryInfos = entryInfos;
            RepeatedEntry = repeatedEntry;
        }
        
        public List<EntryInfo> EntryInfos { get; }
        
        public EntryInfo RepeatedEntry { get; }
    }
}