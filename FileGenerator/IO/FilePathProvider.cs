using System;
using System.IO;

namespace FileGenerator.IO
{
    internal sealed class FilePathProvider : IFilePathProvider
    {
        public FilePathProvider()
        {
            Directory.CreateDirectory("files");
        }
        
        public string GetPath()
            => Path.Combine("files", $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}-data.txt");
    }
}