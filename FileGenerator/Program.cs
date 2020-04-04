using System;
using Autofac;
using FileGenerator.Logging;
using NLog;

namespace FileGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            LogConfigurator.Configure();
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                logger.Info("Start application");

                logger.Info("Initialize container");
                using var container = InitializeContainer();
                
                var bootstrapper = container.Resolve<IBootstrapper>();
                var result = bootstrapper.Start(args);
                
                if (result == GenerationResult.Success)
                {
                    logger.Info("Finished with result: {result}", result);
                }
                else
                {
                    logger.Error("Finished with result: {result}", result);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Stopped program because of exception");
            }
        }

        private static IContainer InitializeContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutofacModule>();
            return builder.Build();
        }
    }
}