﻿using System;
using System.Diagnostics;
using System.IO;
using Autofac;
using NLog;
using TextFileSorter.Logging;
using TextFileSorter.Sorting;
using TextFileSorter.Validation;

namespace TextFileSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            // FOR DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            const string sourceFolder = @"C:\Users\VKorelin\source\repos\TextFileSorter\TestData";
            args = new[] {Path.Combine(sourceFolder, "data_1kb.txt")};
            
            LogConfigurator.Configure();
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                logger.Info("Start application");

                logger.Info("Initialize container");
                using var container = InitializeContainer();

                logger.Info("Validate arguments");
                var argumentsValidator = container.Resolve<IArgumentsValidator>();
                if (!argumentsValidator.IsValid(args, out var fileName))
                {
                    return;
                }
                logger.Info("Arguments are valid");
                
                var externalMergeSorter = container.Resolve<IExternalMergeSorter>();
                var result = externalMergeSorter.Sort(fileName);
                if (result)
                {
                    logger.Info("Finished successfully");
                }
                else
                {
                    logger.Error("Finished with errors");
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Stopped program because of exception");
            }
        }

        private static IContainer InitializeContainer()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AutofacModule>();
            return containerBuilder.Build();
        }
    }
}