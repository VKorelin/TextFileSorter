using System.Collections.Generic;

namespace TextFileSorter.Sorting
{
    public interface IChunkMergeService
    {
        void MergeChunks(IList<string> chunkNames, string outputFileName);
    }
}