using System;
using System.Collections.Generic;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    internal sealed class ChunkGenerator : IChunkGenerator
    {
        private const int MaxEntrySize = 100;

        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IRandomStringGenerator _randomStringGenerator;
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        public ChunkGenerator(
            IEncodingInfoProvider encodingInfoProvider,
            IRandomNumberGenerator randomNumberGenerator,
            IRandomStringGenerator randomStringGenerator)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _randomStringGenerator = randomStringGenerator;
            _encodingInfoProvider = encodingInfoProvider;
        }

        public string GenerateNext(long bufferSize)
        {
            var entryInfos = GetRandomEntryLengths(bufferSize);

            var entries = new List<string>();

            foreach (var entryInfo in entryInfos)
            {
                var number = _randomNumberGenerator.Generate(0, (int) Math.Pow(10, entryInfo.NumberLength));
                var line = _randomStringGenerator.Generate(entryInfo.LineLength);
                
                entries.Add(entryInfo.BuildEntry(number, line));
            }

            return string.Join("\r\n", entries);
        }

        private IEnumerable<EntryInfo> GetRandomEntryLengths(long bufferSize)
        {
            var entryInfos = new List<EntryInfo>();
            var diff = _encodingInfoProvider.GetStringLength(bufferSize);

            while (true)
            {
                var nextLength = _randomNumberGenerator.Generate(EntryInfo.MinLength, MaxEntrySize);
                if (nextLength < diff - EntryInfo.MinLength)
                {
                    entryInfos.Add(CreateEntryInfo(nextLength));
                    diff -= nextLength;
                }
                else
                {
                    entryInfos.Add(CreateEntryInfo((int) diff));
                    break;
                }
            }

            return entryInfos;
        }

        private static EntryInfo CreateEntryInfo(int totalLength)
        {
            var length = totalLength - EntryInfo.ServiceLength;
            
            //Max number size that can be generated is MaxNumberSize. If entry line is short take only half of size for number (e.g. '4. a')
            var numberLength = Math.Min(length / 2, EntryInfo.MaxNumberLength);

            return new EntryInfo(numberLength, length - numberLength);
        }
    }
}