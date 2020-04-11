using System;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Moq;
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
            _configurationProviderMock.SetupGet(x => x.ThreadCount).Returns(Environment.ProcessorCount / 2);
            _configurationProviderMock.SetupGet(x => x.Encoding).Returns(Encoding.Unicode);
            
            _containerBuilder.RegisterInstance(_configurationProviderMock.Object).As<IConfigurationProvider>().SingleInstance();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_sortedFileName))
                File.Delete(_sortedFileName);
            
            _fileGenerator?.Dispose();
            _container?.Dispose();
        }

        private string GenerateFile(long size, Encoding encoding = null)
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
            for (var i = 0; i < entries.Length - 2; i++)
            {
                var entry = entries[i];
                var nextEntry = entries[i + 1];

                var linesCompareResult = string.CompareOrdinal(entry.Line, nextEntry.Line);
                try
                {
                    linesCompareResult.ShouldBeLessThanOrEqualTo(0);

                    if (linesCompareResult == 0)
                    {
                        entry.Number.ShouldBeLessThanOrEqualTo(nextEntry.Number);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
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
        
        /// <summary>
        /// ToDo: drop after debugging
        /// </summary>
        //[TestCase(1024 * 1024 * 128)]
        //public void DEBUG(long fileSize)
        //{
        //    const string file = @"C:\Users\VKorelin\source\repos\TextFileSorter\TextFileSorter.IntegrationTests\bin\Debug\netcoreapp3.1\data\file.txt";
        //    
        //    var sorter = CreateSorter(Encoding.Unicode);
        //    sorter.Sort(file).ShouldBeTrue();
        //    
        //    var sortedFile = AssertSortedFileExists(file, fileSize);
        //    AssertFileSorted(sortedFile);
        //}
    }
}