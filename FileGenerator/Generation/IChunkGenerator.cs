namespace FileGenerator.Generation
{
    public interface IChunkGenerator
    {
        string GenerateNext(long bufferSize);
    }
}