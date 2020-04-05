using System;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    public class Generator : IGenerator
    {
        private const int DefaultBufferSize = 1024 * 1024 * 8;

        private readonly IChunkGenerator _chunkGenerator;
        private readonly IEncodingInfoProvider _encodingInfoProvider;
        private readonly Func<string, IFileWriter> _fileWrapperFactory;

        public Generator(
            IChunkGenerator chunkGenerator, 
            IEncodingInfoProviderFactory encodingInfoProviderFactory, 
            Func<string, IFileWriter> fileWrapperFactory)
        {
            _chunkGenerator = chunkGenerator;
            _fileWrapperFactory = fileWrapperFactory;
            _encodingInfoProvider = encodingInfoProviderFactory.Create();
        }

        public void Generate(long fileSize)
        {
            long currentFileSize = 0;

            using (var fileWriter = _fileWrapperFactory.Invoke("data.txt"))
            {
                while (currentFileSize < fileSize)
                {
                    var bufferSize = Math.Min(fileSize - currentFileSize, DefaultBufferSize);
                    var chunk = _chunkGenerator.GenerateNext(bufferSize);
                    fileWriter.WriteChunk(chunk);
                    currentFileSize += _encodingInfoProvider.CalculateSize(chunk);
                }
            }
        }
    }
}