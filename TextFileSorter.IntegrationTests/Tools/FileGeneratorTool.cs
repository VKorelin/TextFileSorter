using System;
using System.IO;
using System.Text;
using Autofac;
using FileGenerator;
using FileGenerator.Configuration;
using FileGenerator.IO;
using Moq;
using Shouldly;

namespace TextFileSorter.IntegrationTests.Tools
{
    public class FileGeneratorTool : IDisposable
    {
        private const string FileName = "data\\file.txt";
        
        private readonly ContainerBuilder _containerBuilder;
        private readonly Mock<IConfigurationProvider> _configurationProviderMock;
        private IContainer _container;

        public FileGeneratorTool()
        {
            Directory.CreateDirectory("data");
            
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterModule<FileGenerator.AutofacModule>();

            var fileNameProviderMock = new Mock<IFileNameProvider>();
            fileNameProviderMock.Setup(x => x.GetPath()).Returns(FileName);
            _containerBuilder.RegisterInstance(fileNameProviderMock.Object).As<IFileNameProvider>().SingleInstance();

            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _containerBuilder.RegisterInstance(_configurationProviderMock.Object).As<IConfigurationProvider>().SingleInstance();
            _configurationProviderMock.SetupGet(x => x.ThreadsCount).Returns(Environment.ProcessorCount);
            _configurationProviderMock.SetupGet(x => x.DefaultBufferSize).Returns(1024 * 1024 * 8);
        }

        private IBootstrapper CreateBootstrapper(Encoding encoding)
        {
            _configurationProviderMock.SetupGet(x => x.Encoding).Returns(encoding);
            _container = _containerBuilder.Build();
            return _container.Resolve<IBootstrapper>();
        }
        
        private static void AssertFileExists(long size)
        {
            File.Exists(FileName).ShouldBeTrue();

            var info = new FileInfo(FileName);
            info.Length.ShouldBe(size, $"FileSize should be {size} but was {info.Length}");
        }

        public string GenerateFile(long size, Encoding encoding)
        {
            var bootstrapper = CreateBootstrapper(encoding);
            var result = bootstrapper.Start(new[] {size.ToString()});
            result.ShouldBe(GenerationResult.Success);
            AssertFileExists(size);
            return FileName;
        }

        public void Dispose()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);
            
            _container?.Dispose();
        }
    }
}