using FileGenerator.Generation;
using FileGenerator.IO;

namespace FileGenerator.Validation
{
    internal sealed class ArgumentsValidator : IArgumentsValidator
    {
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        public ArgumentsValidator(IEncodingInfoProvider encodingInfoProvider)
        {
            _encodingInfoProvider = encodingInfoProvider;
        }
        
        public bool IsValid(string[] args, out long fileSize)
        {
            fileSize = -1;
            if (args == null || args.Length == 0)
            {
                return false;
            }

            // Generated file should contain at least two duplicated rows
            return long.TryParse(args[0], out fileSize) && _encodingInfoProvider.GetStringLength(fileSize) > EntryInfo.MinLength * 2;
        }
    }
}