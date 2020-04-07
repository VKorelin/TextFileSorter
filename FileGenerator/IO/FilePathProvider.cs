using System;
using System.IO;

namespace FileGenerator.IO
{
    ///<inheritdoc/>
    internal sealed class FilePathProvider : IFilePathProvider
    {
        public FilePathProvider()
        {
            Directory.CreateDirectory("files");
        }
        
        ///<inheritdoc/>
        public string GetPath()
            => Path.Combine("files", $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}-data.txt");
    }
}