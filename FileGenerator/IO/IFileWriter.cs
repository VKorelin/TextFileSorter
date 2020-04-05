using System;

namespace FileGenerator.IO
{
    public interface IFileWriter : IDisposable
    {
        void WriteChunk(string chunk);
    }
}