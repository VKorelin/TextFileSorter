using System;
using FileGenerator.Generation;
using FileGenerator.IO;
using FileGenerator.Validation;
using NLog;

namespace FileGenerator
{
    internal sealed class Bootstrapper : IBootstrapper
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IArgumentsValidator _argumentsValidator;
        private readonly IGenerator _generator;
        private readonly IEncodingInfoProvider _encodingInfoProvider;

        public Bootstrapper(IArgumentsValidator argumentsValidator, IGenerator generator, IEncodingInfoProvider encodingInfoProvider)
        {
            _argumentsValidator = argumentsValidator;
            _generator = generator;
            _encodingInfoProvider = encodingInfoProvider;
        }
        
        public GenerationResult Start(string[] args)
        {
            Logger.Info("Validate arguments");
            if (!_argumentsValidator.IsValid(args, out var fileSize))
            {
                Logger.Error($"FileSize should be specified as first argument and be more than {_encodingInfoProvider.GetBytesCount(12)}");
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