namespace FileGenerator
{
    internal interface IBootstrapper
    {
        GenerationResult Start(string[] args);
    }
}