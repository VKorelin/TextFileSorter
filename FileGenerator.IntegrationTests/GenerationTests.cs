using System.IO;
using Autofac;
using FileGenerator.Logging;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.IntegrationTests
{
    public class GenerationTests
    {
        private const string DirectoryName = "files";

        private IContainer _container;
        private IBootstrapper _bootstrapper;
        
        [SetUp]
        public void Setup()
        {
            LogConfigurator.Configure();
            
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AutofacModule>();
            _container = containerBuilder.Build();

            _bootstrapper = _container.Resolve<IBootstrapper>();

            Directory.CreateDirectory(DirectoryName);
        }

        [TearDown]
        public void TearDown()
        {
            if(Directory.Exists(DirectoryName))
                Directory.Delete(DirectoryName, true);
            
            _container?.Dispose();
        }

        [Test]
        public void CannotGenerateFileWithInvalidArguments()
        {
            var result = _bootstrapper.Start(new[] {"1024sd", "1024"});
            result.ShouldBe(GenerationResult.ArgumentsInvalid);
        }

        [Test]
        public void GeneratesFileForDebug()
        {
            var fileSize = 1024;
            //var fileSize = 1024 * 16;
            //var fileSize = 1024 * 1024 * 32;
            //const long fileSize = 10737418240; //1024 * 1024 * 1024 * 10;
            _bootstrapper.Start(new[] {fileSize.ToString()});
        }
    }
}