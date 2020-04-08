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

        private readonly IChunkGenerator _chunkGenerator;
        private readonly Func<string, IFileWriter> _fileWriterFactory;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        public Generator(
            IChunkGenerator chunkGenerator, 
            Func<string, IFileWriter> fileWriterFactory,
            IFileNameProvider fileNameProvider,
            IEncodingInfoProvider encodingInfoProvider)
        {
            _chunkGenerator = chunkGenerator;
            _fileWriterFactory = fileWriterFactory;
            _fileNameProvider = fileNameProvider;
            _encodingInfoProvider = encodingInfoProvider;
        }
        
        ///<inheritdoc/>
        public void Generate(long fileSize)
        {
            var fileName = _fileNameProvider.GetPath();
            Logger.Info("File name is {fileName}", fileName);
            
            using (var fileWriter = _fileWriterFactory.Invoke(fileName))
            {
                fileSize = AdjustFileSize(fileSize);
                long currentFileSize = 0;
                
                do
                {
                    var chunkSize = CalculateChunkSize(fileSize - currentFileSize);
                    currentFileSize += chunkSize;
                    var chunk = _chunkGenerator.GenerateNext(chunkSize);
                    fileWriter.Write(chunk);
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