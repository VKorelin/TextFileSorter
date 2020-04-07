using System;
using System.Collections.Generic;
using FileGenerator.Domain;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    internal sealed class ChunkInfoBuilder : IChunkInfoBuilder
    {
        private const int MaxEntrySize = 100;

        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IEncodingInfoProvider _encodingInfoProvider;
        
        public ChunkInfoBuilder(IRandomNumberGenerator randomNumberGenerator, IEncodingInfoProvider encodingInfoProvider)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _encodingInfoProvider = encodingInfoProvider;
        }
        
        public ChunkInfo Build(long bufferSize)
        {
            var entryInfos = new List<EntryInfo>();
            var diff = _encodingInfoProvider.GetStringLength(bufferSize);

            //First entry of chunk should be repeated
            var repeatedEntryLength = _randomNumberGenerator.Generate(EntryInfo.MinLength, MaxEntrySize);
            if (repeatedEntryLength * 2 >= diff - EntryInfo.MinLength)
            {
                // In this case chunk will be small and contain only row and repeated row
                return new ChunkInfo(new List<EntryInfo>(), CreateEntryInfo((int) diff / 2, true));
            }

            var repeatedEntryInfo = CreateEntryInfo(repeatedEntryLength, true);
            diff -= repeatedEntryLength * 2;

            while (diff > 0)
            {
                var nextLength = _randomNumberGenerator.Generate(EntryInfo.MinLength, MaxEntrySize);
                var totalLength = nextLength < diff - EntryInfo.MinLength ? nextLength : diff;
                entryInfos.Add(CreateEntryInfo((int) totalLength, false));
                diff -= totalLength;
            }

            return new ChunkInfo(entryInfos, repeatedEntryInfo);
        }
        
        private static EntryInfo CreateEntryInfo(int totalLength, bool isDuplicated)
        {
            var length = totalLength - EntryInfo.ServiceLength;

            //Max number size that can be generated is MaxNumberSize. If entry line is short take only half of size for number (e.g. '4. a')
            var numberLength = Math.Min(length / 2, EntryInfo.MaxNumberLength);

            return new EntryInfo(numberLength, length - numberLength, isDuplicated);
        }
    }
}