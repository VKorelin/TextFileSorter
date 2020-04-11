using System;
using System.IO;
using System.Reflection;
using FileGenerator.Configuration;

namespace FileGenerator.IO
{
    ///<inheritdoc/>
    internal sealed class FileNameProvider : IFileNameProvider
    {
        private readonly string _outputFolder;
        
        public FileNameProvider(IConfigurationProvider configurationProvider)
        {
            var outputPath = configurationProvider.OutputFolder;

            var assembly = Assembly.GetExecutingAssembly();
            var dir = Path.GetDirectoryName(assembly.FullName);
            
            _outputFolder = Path.IsPathRooted(outputPath) ? outputPath : Path.Combine(dir, outputPath);

            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }
        }
        
        ///<inheritdoc/>
        public string GetPath()
            => Path.Combine(_outputFolder, $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}-data.txt");
    }
}