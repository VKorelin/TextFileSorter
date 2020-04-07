using System.Collections.Generic;

namespace FileGenerator.Domain
{
    /// <summary>
    /// Information about piece of data (chunk) that will be written in file
    /// </summary>
    public class ChunkInfo
    {
        /// <summary>
        /// Create chunk info
        /// </summary>
        /// <param name="entryInfos">Information about entries in the chunk</param>
        /// <param name="repeatedEntry">Information about entry that should be repeated in file</param>
        public ChunkInfo(List<EntryInfo> entryInfos, EntryInfo repeatedEntry)
        {
            EntryInfos = entryInfos;
            RepeatedEntry = repeatedEntry;
        }
        
        /// <summary>
        /// Information about entries in the chunk
        /// </summary>
        public List<EntryInfo> EntryInfos { get; }
        
        /// <summary>
        /// Information about entry that should be repeated in file
        /// </summary>
        public EntryInfo RepeatedEntry { get; }
    }
}