﻿using System;
using System.Collections.Generic;
using FileGenerator.Domain;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    ///<inheritdoc/>
    internal sealed class ChunkInfoBuilder : IChunkInfoBuilder
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IEncodingInfoProvider _encodingInfoProvider;
        
        public ChunkInfoBuilder(IRandomNumberGenerator randomNumberGenerator, IEncodingInfoProvider encodingInfoProvider)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _encodingInfoProvider = encodingInfoProvider;
        }
        
        ///<inheritdoc/>
        public ChunkInfo Build(long chunkSize)
        {
            var entryInfos = new List<EntryInfo>();
            var diff = _encodingInfoProvider.GetStringLength(chunkSize);

            //First entry of chunk should be repeated
            var repeatedEntryLength = _randomNumberGenerator.Generate(EntryInfo.MinLength, EntryInfo.MaxEntryLength);
            if (repeatedEntryLength * 2 >= diff - EntryInfo.MinLength)
            {
                // In this case chunk will be small and contain only row and repeated row
                return new ChunkInfo(new List<EntryInfo>(), CreateEntryInfo((int) diff / 2));
            }

            var repeatedEntryInfo = CreateEntryInfo(repeatedEntryLength);
            diff -= repeatedEntryLength * 2;

            while (diff > 0)
            {
                var nextLength = _randomNumberGenerator.Generate(EntryInfo.MinLength, EntryInfo.MaxEntryLength);
                var totalLength = nextLength < diff - EntryInfo.MinLength ? nextLength : diff;
                entryInfos.Add(CreateEntryInfo((int) totalLength));
                diff -= totalLength;
            }

            return new ChunkInfo(entryInfos, repeatedEntryInfo);
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