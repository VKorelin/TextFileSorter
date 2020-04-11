using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Autofac;
using FileGenerator.Configuration;
using FileGenerator.Domain;
using FileGenerator.IO;
using FileGenerator.Logging;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.IntegrationTests
{
    [SuppressMessage("ReSharper", "PossibleUnintendedReferenceComparison")]
    public class GenerationTests
    {
        private const string DirectoryName = "data";
        private static readonly string FileName = Path.Combine(DirectoryName, "testData.txt");

        private ContainerBuilder _containerBuilder;
        private IContainer _container;

        [SetUp]
        public void Setup()
        {
            LogConfigurator.Configure();

            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterModule<AutofacModule>();

            var filePathProviderMock = new Mock<IFileNameProvider>();
            filePathProviderMock.Setup(x => x.GetPath()).Returns(FileName);
            _containerBuilder.RegisterInstance(filePathProviderMock.Object).As<IFileNameProvider>().SingleInstance();

            Directory.CreateDirectory(DirectoryName);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(DirectoryName))
                Directory.Delete(DirectoryName, true);

            _container?.Dispose();
        }

        private IBootstrapper CreateInstance()
        {
            _container = _containerBuilder.Build();
            return _container.Resolve<IBootstrapper>();
        }

        private static void AssertFileExists(long size)
        {
            File.Exists(FileName).ShouldBeTrue();

            var info = new FileInfo(FileName);
            info.Length.ShouldBe(size, $"FileSize should be {size} but was {info.Length}");
        }

        [Test]
        public void CannotGenerateFileWithInvalidArguments()
        {
            var bootstrapper = CreateInstance();

            var result = bootstrapper.Start(new[] {"1024sd", "1024"});
            result.ShouldBe(GenerationResult.ArgumentsInvalid);
        }

        [Test]
        public void Generates1KbFileInUnicode()
        {
            const long fileSize = 1024;

            var configurationProvider = Mock.Of<IConfigurationProvider>(x => x.Encoding == Encoding.Unicode);
            _containerBuilder.RegisterInstance(configurationProvider).As<IConfigurationProvider>().SingleInstance();

            var bootstrapper = CreateInstance();

            bootstrapper.Start(new[] {fileSize.ToString()});

            AssertFileExists(fileSize);
        }

        [Test]
        public void Generates1KbFileInUtf8()
        {
            const long fileSize = 1024;

            var configurationProvider = Mock.Of<IConfigurationProvider>(x => x.Encoding == Encoding.UTF8);
            _containerBuilder.RegisterInstance(configurationProvider).As<IConfigurationProvider>().SingleInstance();

            var bootstrapper = CreateInstance();

            bootstrapper.Start(new[] {fileSize.ToString()});

            AssertFileExists(fileSize);
        }
        
        [Test]
        public void Generates1GbFileInUnicode()
        {
            const long fileSize = 1073741824; //1024 * 1024 * 1024;
            
            var configurationProvider = Mock.Of<IConfigurationProvider>(x => x.Encoding == Encoding.Unicode);
            _containerBuilder.RegisterInstance(configurationProvider).As<IConfigurationProvider>().SingleInstance();

            var bootstrapper = CreateInstance();

            bootstrapper.Start(new[] {fileSize.ToString()});
            
            AssertFileExists(fileSize);
        }
        
        [Test]
        public void Generates1GbFileInUtf8()
        {
            const long fileSize = 1073741824; //1024 * 1024 * 1024;

            var configurationProvider = Mock.Of<IConfigurationProvider>(x => x.Encoding == Encoding.Unicode);
            _containerBuilder.RegisterInstance(configurationProvider).As<IConfigurationProvider>().SingleInstance();

            var bootstrapper = CreateInstance();

            bootstrapper.Start(new[] {fileSize.ToString()});
            
            AssertFileExists(fileSize);
        }

        [Test]
        [Timeout(1000 * 60 * 3)]
        public void Generates10GbFileInUnicode()
        {
            const long fileSize = 10737418240; //1024 * 1024 * 1024 * 10;

            var configurationProvider = Mock.Of<IConfigurationProvider>(x => x.Encoding == Encoding.Unicode);
            _containerBuilder.RegisterInstance(configurationProvider).As<IConfigurationProvider>().SingleInstance();

            var bootstrapper = CreateInstance();

            bootstrapper.Start(new[] {fileSize.ToString()});

            AssertFileExists(fileSize);
        }

        [Test]
        [Timeout(1000 * 60 * 6)]
        public void Generates10GbFileInUtf8()
        {
            const long fileSize = 10737418240; //1024 * 1024 * 1024 * 10;

            var configurationProvider = Mock.Of<IConfigurationProvider>(x => x.Encoding == Encoding.UTF8);
            _containerBuilder.RegisterInstance(configurationProvider).As<IConfigurationProvider>().SingleInstance();

            var bootstrapper = CreateInstance();

            bootstrapper.Start(new[] {fileSize.ToString()});

            AssertFileExists(fileSize);
        }

        [Test]
        public void GeneratesFileWithCorrectStructure()
        {
            const long fileSize = 1024 * 1024 * 32; //32Mb

            var configurationProvider = Mock.Of<IConfigurationProvider>(x => x.Encoding == Encoding.UTF8);
            _containerBuilder.RegisterInstance(configurationProvider).As<IConfigurationProvider>().SingleInstance();

            var bootstrapper = CreateInstance();

            bootstrapper.Start(new[] {fileSize.ToString()});

            AssertFileExists(fileSize);

            var entries = File.ReadAllLines(FileName);
            foreach (var entry in entries)
            {
                var entryArr = entry.Split(". ");
                entryArr.Length.ShouldBe(2);

                var number = int.Parse(entryArr[0]);
                number.ShouldBeInRange(1, (int) Math.Pow(10, EntryInfo.MaxNumberLength));

                var lineLength = entryArr[1].Length;
                lineLength.ShouldBeGreaterThanOrEqualTo(1);
                lineLength.ShouldBeLessThanOrEqualTo(EntryInfo.MaxEntryLength - entryArr.Length - 2);
            }
        }
    }
}