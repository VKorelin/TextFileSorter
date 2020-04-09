using System;

namespace TextFileSorter.Sorting
{
    public interface IChunkFileReader : IDisposable
    {
        ReadChunkResult ReadNextChunk(int shift);
    }
}