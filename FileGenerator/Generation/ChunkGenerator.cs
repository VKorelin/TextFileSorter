using System.Collections.Generic;
using System.Text;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    internal sealed class ChunkGenerator : IChunkGenerator
    {
        private const int MaxEntrySize = 100;

        private readonly IEntryGenerator _entryGenerator;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        public ChunkGenerator(
            IEntryGenerator entryGenerator, 
            IEncodingInfoProvider encodingInfoProvider,
            IRandomNumberGenerator randomNumberGenerator)
        {
            _entryGenerator = entryGenerator;
            _randomNumberGenerator = randomNumberGenerator;
            _encodingInfoProvider = encodingInfoProvider;
        }

        public string GenerateNext(long bufferSize)
        {
            var stringBuilder = new StringBuilder();
            var randomLengths = GetRandomEntryLengths(bufferSize);

            foreach (var length in randomLengths)
            {
                stringBuilder.Append(_entryGenerator.Generate(length));
            }
            
            return stringBuilder.Remove(stringBuilder.Length - 2, 2).ToString();
        }

        private IEnumerable<int> GetRandomEntryLengths(long bufferSize)
        {
            var lengths = new List<int>();
            var diff = _encodingInfoProvider.GetStringLength(bufferSize);

            while (true)
            {
                var nextLength = _randomNumberGenerator.Generate(EntryInfo.MinLength, MaxEntrySize);
                if (nextLength < diff - EntryInfo.MinLength)
                {
                    lengths.Add(nextLength);
                    diff -= nextLength;
                }
                else
                {
                    lengths.Add((int) diff);
                    break;
                }
            }

            return lengths;
        }
    }
}