using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace FileGenerator.Configuration
{
    ///<inheritdoc/>
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
        }

        ///<inheritdoc/>
        public Encoding Encoding { get; }
    }
}