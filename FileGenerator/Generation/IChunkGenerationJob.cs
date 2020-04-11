namespace FileGenerator.Generation
{
    public interface IChunkGenerationJob
    {
        /// <summary>
        /// Start job for chunks generation
        /// </summary>
        void Start();

        /// <summary>
        /// Add chunk size for generation queue
        /// </summary>
        /// <param name="chunkSize">Size of chunk</param>
        void AddNext(long chunkSize);
        
        /// <summary>
        /// Stop generation job
        /// </summary>
        void Stop();
    }
}