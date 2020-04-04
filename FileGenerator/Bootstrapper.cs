using System;
using FileGenerator.Generation;
using FileGenerator.Validation;
using NLog;

namespace FileGenerator
{
    internal sealed class Bootstrapper : IBootstrapper
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IArgumentsValidator _argumentsValidator;
        private readonly IGenerator _generator;

        public Bootstrapper(IArgumentsValidator argumentsValidator, IGenerator generator)
        {
            _argumentsValidator = argumentsValidator;
            _generator = generator;
        }
        
        public GenerationResult Start(string[] args)
        {
            Logger.Info("Validate arguments");
            if (!_argumentsValidator.IsValid(args, out var fileSize))
            {
                Logger.Error("FileSize should be specified as first argument");
                return GenerationResult.ArgumentsInvalid;
            }

            try
            {
                _generator.Generate(fileSize);
                Logger.Info("File generated");
                return GenerationResult.Success;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error during file generation");
                return GenerationResult.GenerationError;
            }
        }
    }
}