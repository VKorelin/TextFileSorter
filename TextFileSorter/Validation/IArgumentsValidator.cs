namespace TextFileSorter.Validation
{
    internal interface IArgumentsValidator
    {
        bool IsValid(string[] args, out string fileName);
    }
}