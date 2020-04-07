using System;
using FileGenerator.Domain;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    public class Generator : IGenerator
    {
        private const int DefaultBufferSize = 1024 * 1024 * 8;

        private readonly IChunkGenerator _chunkGenerator;
        private readonly Func<string, IFileWriter> _fileWrapperFactory;
        private readonly IFilePathProvider _filePathProvider;
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        public Generator(
            IChunkGenerator chunkGenerator, 
            Func<string, IFileWriter> fileWrapperFactory,
            IFilePathProvider filePathProvider,
            IEncodingInfoProvider encodingInfoProvider)
        {
            _chunkGenerator = chunkGenerator;
            _fileWrapperFactory = fileWrapperFactory;
            _filePathProvider = filePathProvider;
            _encodingInfoProvider = encodingInfoProvider;
        }

        public void Generate(long fileSize)
        {
            using (var fileWriter = _fileWrapperFactory.Invoke(_filePathProvider.GetPath()))
            {
                long currentFileSize = 0;
                var isLastChunk = false;
                do
                {
                    var bufferSize = CalculateBufferSize(fileSize - currentFileSize, ref isLastChunk);
                    currentFileSize += bufferSize;
                    var chunk = _chunkGenerator.GenerateNext(bufferSize);
                    fileWriter.WriteChunk(isLastChunk ? TruncateChunk(chunk) : chunk);
                } while (!isLastChunk);
            }
        }

        private long CalculateBufferSize(long freeSpace, ref bool isLastChunk)
        {
            if (DefaultBufferSize <= freeSpace - DefaultBufferSize)
                return DefaultBufferSize;

            isLastChunk = true;
            return freeSpace + _encodingInfoProvider.GetBytesCount(EntryInfo.NewLineEnding.Length);
        }

        private static string TruncateChunk(string chunk) 
            => chunk.EndsWith(EntryInfo.NewLineEnding) 
                ? chunk.Substring(0, chunk.Length - EntryInfo.NewLineEnding.Length) 
                : chunk;
    }
}