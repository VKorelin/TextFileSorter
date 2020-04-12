using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace TextFileSorter.Configuration
{
    internal sealed class ConfigurationProvider : IConfigurationProvider
    {
        public ConfigurationProvider()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var encodingStr = config["Encoding"];
            Encoding = encodingStr.ToLowerInvariant() switch
            {
                "unicode" => Encoding.Unicode,
                "utf-8" => Encoding.UTF8,
                _ => throw new Exception($"Unsupported encoding type: {encodingStr}")
            };
            
            OutputFolder = config["OutputFolder"];
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }
        
        public Encoding Encoding { get; }
        
        public string OutputFolder { get; }

        ///<inheritdoc/>
        public long RamLimit => 1024 * 1024 * 512;

        ///<inheritdoc/>
        public int ThreadCount => Environment.ProcessorCount;

        ///<inheritdoc/>
        public int MexChunksNumberInMerge => 8;
    }
}