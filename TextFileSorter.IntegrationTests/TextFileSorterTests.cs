using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Moq;
using NLog;
using NUnit.Framework;
using Shouldly;
using TextFileSorter.Configuration;
using TextFileSorter.IntegrationTests.Tools;
using TextFileSorter.Logging;
using TextFileSorter.Sorting;

namespace TextFileSorter.IntegrationTests
{
    public class TextFileSorterTests
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private FileGeneratorTool _fileGenerator;
        private ContainerBuilder _containerBuilder;
        private IContainer _container;

        private Mock<IConfigurationProvider> _configurationProviderMock;
        private string _sortedFileName = "";

        [SetUp]
        public void Setup()
        {
            LogConfigurator.Configure();

            _fileGenerator = new FileGeneratorTool();

            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterModule<AutofacModule>();

            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _configurationProviderMock.SetupGet(x => x.OutputFolder).Returns("data");
            _configurationProviderMock.SetupGet(x => x.RamLimit).Returns(1024 * 1024 * 512);
            _configurationProviderMock.SetupGet(x => x.ThreadCount).Returns(Environment.ProcessorCount);
            _configurationProviderMock.SetupGet(x => x.Encoding).Returns(Encoding.Unicode);
            _configurationProviderMock.SetupGet(x => x.MexChunksNumberInMerge).Returns(16);

            _containerBuilder.RegisterInstance(_configurationProviderMock.Object).As<IConfigurationProvider>()
                .SingleInstance();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_sortedFileName))
                File.Delete(_sortedFileName);

            _fileGenerator?.Dispose();
            _container?.Dispose();
        }

        private string GenerateFile(long size, Encoding encoding)
        {
            return _fileGenerator?.GenerateFile(size, encoding);
        }

        private IExternalMergeSorter CreateSorter(Encoding encoding)
        {
            _configurationProviderMock.SetupGet(x => x.Encoding).Returns(encoding);

            _container = _containerBuilder.Build();
            return _container.Resolve<IExternalMergeSorter>();
        }

        private string AssertSortedFileExists(string sourceFileName, long size)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFileName);
            var directoryName = Path.GetDirectoryName(sourceFileName);
            _sortedFileName = Path.Combine(directoryName, $"{fileNameWithoutExtension}_sorted.txt");

            File.Exists(_sortedFileName).ShouldBeTrue();
            var fi = new FileInfo(_sortedFileName);
            fi.Length.ShouldBe(size);

            return _sortedFileName;
        }

        private static void AssertFileSorted(string fileName)
        {
            var entries = File.ReadAllLines(fileName).Select(Entry.Build).ToArray();
            for (var i = 0; i < entries.Length - 1; i++)
            {
                var entry = entries[i];
                var nextEntry = entries[i + 1];
                
                entry.CompareTo(nextEntry).ShouldBeLessThanOrEqualTo(0);
            }
        }

        [TestCase(1024)]
        [TestCase(1024 * 1024)]
        [TestCase(1024 * 1024 * 64)]
        [TestCase(1024 * 1024 * 128)]
        [TestCase(1024 * 1024 * 256)]
        [TestCase(1024 * 1024 * 512)]
        public void SortsFileInUnicode(long fileSize)
        {
            var file = GenerateFile(fileSize, Encoding.Unicode);

            var sorter = CreateSorter(Encoding.Unicode);
            sorter.Sort(file).ShouldBeTrue();

            var sortedFile = AssertSortedFileExists(file, fileSize);
            AssertFileSorted(sortedFile);
        }

        [TestCase(1024)]
        [TestCase(1024 * 1024)]
        [TestCase(1024 * 1024 * 64)]
        [TestCase(1024 * 1024 * 128)]
        [TestCase(1024 * 1024 * 256)]
        [TestCase(1024 * 1024 * 512)]
        public void SortsFileInUtf8(long fileSize)
        {
            var file = GenerateFile(fileSize, Encoding.UTF8);

            var sorter = CreateSorter(Encoding.UTF8);
            sorter.Sort(file).ShouldBeTrue();

            var sortedFile = AssertSortedFileExists(file, fileSize);
            AssertFileSorted(sortedFile);
        }

        [Test]
        public void Sorts1GbFileInUnicode()
        {
            const int fileSize = 1024 * 1024 * 1024;

            var file = GenerateFile(fileSize, Encoding.Unicode);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sorter = CreateSorter(Encoding.Unicode);
            sorter.Sort(file).ShouldBeTrue();

            stopWatch.Stop();
            Logger.Info($"File sorted in {stopWatch.Elapsed}");

            AssertSortedFileExists(file, fileSize);
        }

        [Test]
        public void Sorts1GbFileInUtf8()
        {
            const int fileSize = 1024 * 1024 * 1024;

            var file = GenerateFile(fileSize, Encoding.UTF8);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sorter = CreateSorter(Encoding.UTF8);
            sorter.Sort(file).ShouldBeTrue();

            stopWatch.Stop();
            Logger.Info($"File sorted in {stopWatch.Elapsed}");

            AssertSortedFileExists(file, fileSize);
        }

        [Test]
        public void Sorts10GbFileInUnicode()
        {
            const long fileSize = 10737418240; //1024 * 1024 * 1024 * 10;

            var file = GenerateFile(fileSize, Encoding.Unicode);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sorter = CreateSorter(Encoding.Unicode);
            sorter.Sort(file).ShouldBeTrue();

            stopWatch.Stop();
            Logger.Info($"File sorted in {stopWatch.Elapsed}");

            AssertSortedFileExists(file, fileSize);
        }

        [Test]
        public void Sorts10GbFileInUtf8()
        {
            const long fileSize = 10737418240; //1024 * 1024 * 1024 * 10;

            var file = GenerateFile(fileSize, Encoding.UTF8);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sorter = CreateSorter(Encoding.UTF8);
            sorter.Sort(file).ShouldBeTrue();

            stopWatch.Stop();
            Logger.Info($"File sorted in {stopWatch.Elapsed}");

            AssertSortedFileExists(file, fileSize);
        }

        [Test]
        public void LetsGoToDrinkSomething()
        {
            const long fileSize = 107374182400; //1024 * 1024 * 1024 * 100;

            var file = GenerateFile(fileSize, Encoding.Unicode);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sorter = CreateSorter(Encoding.Unicode);
            sorter.Sort(file).ShouldBeTrue();

            stopWatch.Stop();
            Logger.Info($"File sorted in {stopWatch.Elapsed}");

            AssertSortedFileExists(file, fileSize);
        }

        [Test]
        public void HowAboutBeer()
        {
            const long fileSize = 107374182400; //1024 * 1024 * 1024 * 100;

            var file = GenerateFile(fileSize, Encoding.UTF8);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var sorter = CreateSorter(Encoding.UTF8);
            sorter.Sort(file).ShouldBeTrue();

            stopWatch.Stop();
            Logger.Info($"File sorted in {stopWatch.Elapsed}");

            AssertSortedFileExists(file, fileSize);
        }
    }
}