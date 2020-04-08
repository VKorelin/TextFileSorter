using System;
using FileGenerator.Domain;
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
                long currentFileSize = 0;
                var isLastChunk = false;
                do
                {
                    var chunkSize = CalculateChunkSize(fileSize - currentFileSize, ref isLastChunk);
                    currentFileSize += chunkSize;
                    var chunk = _chunkGenerator.GenerateNext(chunkSize);
                    fileWriter.Write(isLastChunk ? TruncateChunk(chunk) : chunk);
                } while (!isLastChunk);
            }
        }

        /// <summary>
        /// Calculate size of chunk 
        /// </summary>
        /// <param name="freeSpace">Part of file in bytes that should be filled</param>
        /// <param name="isLastChunk">Indicates if written chunk is last</param>
        /// <returns>Chunk size</returns>
        private long CalculateChunkSize(long freeSpace, ref bool isLastChunk)
        {
            if (DefaultBufferSize <= freeSpace - DefaultBufferSize)
                return DefaultBufferSize;

            isLastChunk = true;
            
            // In the end of each file there is no any new line chars but we should take into account that txt file has additional size
            return freeSpace + _encodingInfoProvider.GetBytesCountInStringLength(EntryInfo.NewLineEnding.Length) - _encodingInfoProvider.AdditionalFileSize;
        }

        /// <summary>
        /// Remove last chars from chunk (new line chars)
        /// </summary>
        /// <param name="chunk">Chunk to be truncated</param>
        /// <returns>Truncated chunk</returns>
        private static string TruncateChunk(string chunk) 
            => chunk.EndsWith(EntryInfo.NewLineEnding) 
                ? chunk.Substring(0, chunk.Length - EntryInfo.NewLineEnding.Length) 
                : chunk;
    }
}