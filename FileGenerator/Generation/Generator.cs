using System;
using FileGenerator.IO;
using NLog;

namespace FileGenerator.Generation
{
    ///<inheritdoc/>
    public class Generator : IGenerator
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Size of chunk
        /// </summary>
        private const int DefaultBufferSize = 1024 * 1024 * 8;

        private readonly Func<IChunkGenerator> _chunkGeneratorFactory;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        public Generator(
            Func<IChunkGenerator> chunkGeneratorFactory,
            IFileNameProvider fileNameProvider,
            IEncodingInfoProvider encodingInfoProvider)
        {
            _chunkGeneratorFactory = chunkGeneratorFactory;
            _fileNameProvider = fileNameProvider;
            _encodingInfoProvider = encodingInfoProvider;
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
        private static long CalculateChunkSize(long freeSpace)
            => DefaultBufferSize <= freeSpace - DefaultBufferSize ? DefaultBufferSize : freeSpace;
    }
}