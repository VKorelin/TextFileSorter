namespace FileGenerator.Validation
{
    public interface IArgumentsValidator
    {
        bool IsValid(string[] args, out long fileSize);
    }
}