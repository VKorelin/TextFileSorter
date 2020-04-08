using System;
using System.IO;
using FileGenerator.Configuration;

namespace FileGenerator.IO
{
    ///<inheritdoc/>
    internal sealed class FileNameProvider : IFileNameProvider
    {
        private readonly string _outputFolder;
        
        public FileNameProvider(IConfigurationProvider configurationProvider)
        {
            _outputFolder = configurationProvider.OutputFolder;

            if (Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }
        }
        
        ///<inheritdoc/>
        public string GetPath()
            => Path.Combine(_outputFolder, $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}-data.txt");
    }
}