using System;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    public class Generator : IGenerator
    {
        private const int DefaultBufferSize = 1024 * 1024 * 8;

        private readonly IChunkGenerator _chunkGenerator;
        private readonly Func<string, IFileWriter> _fileWrapperFactory;
        private readonly IFilePathProvider _filePathProvider;

        public Generator(
            IChunkGenerator chunkGenerator, 
            Func<string, IFileWriter> fileWrapperFactory,
            IFilePathProvider filePathProvider)
        {
            _chunkGenerator = chunkGenerator;
            _fileWrapperFactory = fileWrapperFactory;
            _filePathProvider = filePathProvider;
        }

        public void Generate(long fileSize)
        {
            long currentFileSize = 0;

            using (var fileWriter = _fileWrapperFactory.Invoke(_filePathProvider.GetPath()))
            {
                while (currentFileSize < fileSize)
                {
                    var bufferSize = CalculateBufferSize(fileSize - currentFileSize);
                    var chunk = _chunkGenerator.GenerateNext(bufferSize);
                    fileWriter.WriteChunk(chunk);
                    currentFileSize += bufferSize;
                }
            }
        }

        private static long CalculateBufferSize(long freeSpace) 
            => DefaultBufferSize < freeSpace - DefaultBufferSize ? DefaultBufferSize : freeSpace;
    }
}