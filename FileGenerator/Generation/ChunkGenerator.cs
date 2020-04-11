using System;
using System.Text;
using FileGenerator.Domain;

namespace FileGenerator.Generation
{
    ///<inheritdoc/>
    internal sealed class ChunkGenerator : IChunkGenerator
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IRandomStringGenerator _randomStringGenerator;
        private readonly IChunkInfoBuilder _chunkInfoBuilder;

        public ChunkGenerator(
            IChunkInfoBuilder chunkInfoBuilder,
            IRandomNumberGenerator randomNumberGenerator,
            IRandomStringGenerator randomStringGenerator)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _randomStringGenerator = randomStringGenerator;
            _chunkInfoBuilder = chunkInfoBuilder;
        }

        ///<inheritdoc/>
        public string GenerateNext(long chunkSize)
        {
            var builder = new StringBuilder();
            
            var chunkInfo = _chunkInfoBuilder.Build(chunkSize);
            builder.Clear();

            // Generate first entry that string will be repeated
            var firstNumber = GenerateNumber(chunkInfo.RepeatedEntry.NumberLength);
            var lineToRepeat = _randomStringGenerator.Generate(chunkInfo.RepeatedEntry.LineLength);
            builder.AppendLine(EntryInfo.BuildEntry(firstNumber, lineToRepeat));

            foreach (var entryInfo in chunkInfo.EntryInfos)
            {
                var number = GenerateNumber(entryInfo.NumberLength);
                var line = _randomStringGenerator.Generate(entryInfo.LineLength);
                builder.AppendLine(EntryInfo.BuildEntry(number, line));
            }

            // Generate last entry with repeated string
            var lastNumber = GenerateNumber(chunkInfo.RepeatedEntry.NumberLength);
            builder.AppendLine(EntryInfo.BuildEntry(lastNumber, lineToRepeat));

            return builder.ToString();
        }
        
        private int GenerateNumber(int length)
            => _randomNumberGenerator.Generate((int) Math.Pow(10, length - 1), (int) Math.Pow(10, length));
    }
}