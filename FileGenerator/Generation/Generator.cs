using FileGenerator.Configuration;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    ///<inheritdoc/>
    public class Generator : IGenerator
    {
        private readonly IChunkGenerationJob _chunkGenerationJob;
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        private readonly int _defaultBufferSize;

        public Generator(
            IChunkGenerationJob chunkGenerationJob,
            IEncodingInfoProvider encodingInfoProvider,
            IConfigurationProvider configurationProvider)
        {
            _chunkGenerationJob = chunkGenerationJob;
            _encodingInfoProvider = encodingInfoProvider;
            _defaultBufferSize = configurationProvider.DefaultBufferSize;
        }
        
        ///<inheritdoc/>
        public void Generate(long fileSize)
        {
            fileSize = AdjustFileSize(fileSize);
            long currentFileSize = 0;
            
            _chunkGenerationJob.Start();

            do
            {
                var chunkSize = CalculateChunkSize(fileSize - currentFileSize);
                currentFileSize += chunkSize;
                _chunkGenerationJob.AddNext(chunkSize);
            } while (currentFileSize < fileSize);
            
            _chunkGenerationJob.Stop();
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