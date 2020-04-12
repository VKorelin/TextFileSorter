using System.Collections.Generic;

namespace TextFileSorter.Sorting
{
    public interface IFilesMerger
    {
        string Merge(IList<string> filesToMerge, long ramAvailable);
    }
}