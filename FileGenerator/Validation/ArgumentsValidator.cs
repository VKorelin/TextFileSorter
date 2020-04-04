namespace FileGenerator.Validation
{
    internal sealed class ArgumentsValidator : IArgumentsValidator
    {
        public bool IsValid(string[] args, out long fileSize)
        {
            fileSize = -1;
            if (args == null || args.Length == 0)
            {
                return false;
            }

            return long.TryParse(args[0], out fileSize);
        }
    }
}