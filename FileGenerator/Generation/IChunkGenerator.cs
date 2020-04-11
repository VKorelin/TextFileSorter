using System;

namespace FileGenerator.Generation
{
    /// <summary>
    /// Generate chunk (piece of data to be written to file)
    /// </summary>
    public interface IChunkGenerator : IDisposable
    {
        /// <summary>
        /// Put next size for generation queue
        /// </summary>
        /// <param name="chunkSize">Size of chunk</param>
        void GenerateNext(long chunkSize);
    }
}