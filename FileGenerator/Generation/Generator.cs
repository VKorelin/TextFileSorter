using System;
using FileGenerator.Configuration;
using FileGenerator.IO;
using NLog;

namespace FileGenerator.Generation
{
    ///<inheritdoc/>
    public class Generator : IGenerator
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly Func<IChunkGenerator> _chunkGeneratorFactory;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        private readonly int _defaultBufferSize;

        public Generator(
            Func<IChunkGenerator> chunkGeneratorFactory,
            IFileNameProvider fileNameProvider,
            IEncodingInfoProvider encodingInfoProvider,
            IConfigurationProvider configurationProvider)
        {
            _chunkGeneratorFactory = chunkGeneratorFactory;
            _fileNameProvider = fileNameProvider;
            _encodingInfoProvider = encodingInfoProvider;
            _defaultBufferSize = configurationProvider.DefaultBufferSize;
        }
        
        ///<inheritdoc/>
        public void Generate(long fileSize)
        {
            var fileName = _fileNameProvider.GetPath();
            Logger.Info("File name is {fileName}", fileName);

            fileSize = AdjustFileSize(fileSize);
            long currentFileSize = 0;

            using (var chunkGenerator = _chunkGeneratorFactory.Invoke())
            {
                do
                {
                    var chunkSize = CalculateChunkSize(fileSize - currentFileSize);
                    currentFileSize += chunkSize;
                    chunkGenerator.GenerateNext(chunkSize);
                } while (currentFileSize < fileSize);
            }
        }

        /// <summary>
        /// Take into account that txt file has additional size
        /// </summary>
        /// <returns>Adjusted file size</returns>
        private long AdjustFileSize(long size)
            => size - _encodingInfoProvider.AdditionalFileSize;

        /// <summary>
        /// Calculate size of chunk 
        /// </summary>
        /// <param name="freeSpace">Part of file in bytes that should be filled</param>
        /// <returns>Chunk size</returns>
        private long CalculateChunkSize(long freeSpace)
            => _defaultBufferSize <= freeSpace - _defaultBufferSize ? _defaultBufferSize : freeSpace;
    }
}