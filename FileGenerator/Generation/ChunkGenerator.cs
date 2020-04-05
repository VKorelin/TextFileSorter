using System;
using System.Collections.Generic;
using System.Text;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    internal sealed class ChunkGenerator : IChunkGenerator
    {
        private readonly IEntryGenerator _entryGenerator;
        private readonly IEncodingInfoProvider _encodingInfoProvider;
        private readonly Random _random;

        public ChunkGenerator(IEntryGenerator entryGenerator, IEncodingInfoProviderFactory encodingInfoProviderFactory)
        {
            _entryGenerator = entryGenerator;
            _encodingInfoProvider = encodingInfoProviderFactory.Create();
            _random = new Random();
        }
        
        public string GenerateNext(long bufferSize)
        {
            var stringBuilder = new StringBuilder();
            var randomLengths = GetRandomLengths(bufferSize);
            
            foreach (var length in randomLengths)
            {
                stringBuilder.AppendLine(_entryGenerator.Generate(length));
            }
            
            return stringBuilder.ToString();
        }
        
        private IEnumerable<long> GetRandomLengths(long bufferSize)
        {
            var lengths = new List<long>();
            var diff = _encodingInfoProvider.GetStringLength(bufferSize);

            while (true)
            {
                var nextLength = _random.Next(2, 100);
                if (nextLength < diff)
                {
                    lengths.Add(nextLength);
                    diff -= nextLength;
                }
                else
                {
                    lengths.Add(diff);
                    break;
                }
            }

            return lengths;
        }
    }
}