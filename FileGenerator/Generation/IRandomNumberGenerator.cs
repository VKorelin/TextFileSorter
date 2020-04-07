namespace FileGenerator.Generation
{
    internal interface IRandomNumberGenerator
    {
        int Generate(int min, int max);
        
        byte[] GenerateNextBytes(long bytes);
    }
}