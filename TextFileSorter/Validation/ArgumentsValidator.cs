using System.IO;
using NLog;

namespace TextFileSorter.Validation
{
    internal sealed class ArgumentsValidator : IArgumentsValidator
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        public bool IsValid(string[] args, out string fileName)
        {
            fileName = null;
            
            if (args == null || args.Length == 0)
            {
                Logger.Error("FileName should be passed as parameter");
                return false;
            }

            fileName = args[0];
            if (!File.Exists(fileName))
            {
                Logger.Error($"File {fileName} does noy exists");
                return false;
            }

            return true;
        }
    }
}