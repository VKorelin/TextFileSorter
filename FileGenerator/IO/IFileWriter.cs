using System;

namespace FileGenerator.IO
{
    /// <summary>
    /// Writer to the file
    /// </summary>
    public interface IFileWriter : IDisposable
    {
        /// <summary>
        /// Write string to the file.
        /// </summary>
        /// <param name="data">String to write</param>
        void Write(string data);
    }
}