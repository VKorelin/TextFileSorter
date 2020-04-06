using System;
using System.Collections.Generic;
using FileGenerator.Domain;
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

            var entries = new List<Entry>();

            Entry entryToRepeat = null;

            foreach (var entryInfo in entryInfos)
            {
                var number = GenerateNumber(entryInfo.NumberLength);
                var line = _randomStringGenerator.Generate(entryInfo.LineLength);

                var entry = new Entry(number, line, entryInfo);
                entries.Add(entry);
                
                if (entryInfo.IsDuplicated)
                {
                    entryToRepeat = entry;
                }
            }

            if (entryToRepeat != null)
            {
                entries.Add(new Entry(
                    GenerateNumber(entryToRepeat.Info.NumberLength),
                    entryToRepeat.Line,
                    new EntryInfo(entryToRepeat.Info.NumberLength, entryToRepeat.Info.LineLength)));
            }

            return string.Join("\r\n", entries);
        }

        private List<EntryInfo> GetRandomEntryLengths(long bufferSize)
        {
            var entryInfos = new List<EntryInfo>();
            var diff = _encodingInfoProvider.GetStringLength(bufferSize);

            //First entry of chunk should be repeated
            var repeatedEntryLength = _randomNumberGenerator.Generate(EntryInfo.MinLength, MaxEntrySize);
            if (repeatedEntryLength * 2 < diff - EntryInfo.MinLength)
            {
                entryInfos.Add(CreateEntryInfo(repeatedEntryLength, true));
                diff -= repeatedEntryLength * 2;
            }
            else
            {
                entryInfos.Add(CreateEntryInfo((int) diff / 2, true));
                return entryInfos;
            }

            while (diff > 0)
            {
                var nextLength = _randomNumberGenerator.Generate(EntryInfo.MinLength, MaxEntrySize);
                var totalLength = nextLength < diff - EntryInfo.MinLength ? nextLength : diff;
                entryInfos.Add(CreateEntryInfo((int) totalLength, false));
                diff -= totalLength;
            }

            return entryInfos;
        }

        private static EntryInfo CreateEntryInfo(int totalLength, bool isDuplicated)
        {
            var length = totalLength - EntryInfo.ServiceLength;

            //Max number size that can be generated is MaxNumberSize. If entry line is short take only half of size for number (e.g. '4. a')
            var numberLength = Math.Min(length / 2, EntryInfo.MaxNumberLength);

            return new EntryInfo(numberLength, length - numberLength, isDuplicated);
        }

        private int GenerateNumber(int length)
            => _randomNumberGenerator.Generate(0, (int) Math.Pow(10, length));
    }
}