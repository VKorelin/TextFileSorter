using System.Collections.Generic;

namespace TextFileSorter.Sorting
{
    public interface ISortedChunksService
    {
        IList<string> CreateSortedChunks();
    }
}