using System;
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
        }
        
        public int ChunkSize => 1024 * 1024 * 100;

        public int ThreadCount => 2;

        public Encoding Encoding { get; }
        
        public string OutputFolder { get; }
    }
}