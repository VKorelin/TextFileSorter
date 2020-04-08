using System;
using FileGenerator.Generation;
using FileGenerator.IO;
using Moq;
using NUnit.Framework;

namespace FileGenerator.Tests.Generation
{
    [TestFixture]
    public class GeneratorTests
    {
        private const string FileName = "fileName";
        private const int AdditionalFileSize = 1;
        
        private Mock<IChunkGenerator> _chunkGeneratorMock;
        private Mock<Func<string, IFileWriter>> _fileWriterFactoryMock;
        private Mock <IFileWriter> _fileWriterMock;
        private Mock<IFileNameProvider> _filePathProviderMock;
        private Mock<IEncodingInfoProvider> _encodingInfoProviderMock;

        [SetUp]
        public void Setup()
        {
            _chunkGeneratorMock = new Mock<IChunkGenerator>();
            _chunkGeneratorMock.Setup(x => x.GenerateNext(It.IsAny<long>())).Returns(string.Empty);
            
            _encodingInfoProviderMock = new Mock<IEncodingInfoProvider>();
            _encodingInfoProviderMock.Setup(x => x.GetBytesCountInStringLength(2)).Returns(2);
            _encodingInfoProviderMock.SetupGet(x => x.AdditionalFileSize).Returns(1);

            _filePathProviderMock = new Mock<IFileNameProvider>();
            _filePathProviderMock.Setup(x => x.GetPath()).Returns(FileName);
            
            
            _fileWriterFactoryMock = new Mock<Func<string, IFileWriter>>();
            _fileWriterMock = new Mock<IFileWriter>();
            _fileWriterFactoryMock.Setup(x => x.Invoke(FileName)).Returns(_fileWriterMock.Object);
        }
        
        private Generator CreateInstance()
            => new Generator(
                _chunkGeneratorMock.Object,
                _fileWriterFactoryMock.Object,
                _filePathProviderMock.Object,
                _encodingInfoProviderMock.Object);

        [Test]
        public void GetsFileNamePromProvider()
        {
            var instance = CreateInstance();
            
            instance.Generate(100);
            
            _filePathProviderMock.Verify(x => x.GetPath(), Times.Once);
        }

        [Test]
        public void CreatesFileWriter()
        {
            var instance = CreateInstance();
            
            instance.Generate(100);
            
            _fileWriterFactoryMock.Verify(x => x.Invoke(FileName), Times.Once);
        }

        [Test]
        public void GeneratesFileWithSmallSingleChunk()
        {
            const int fileSize = 1024;
            _chunkGeneratorMock.Setup(x => x.GenerateNext(fileSize - AdditionalFileSize)).Returns("line\r\n");
            var instance = CreateInstance();
            
            instance.Generate(fileSize);
            
            _chunkGeneratorMock.Verify(x => x.GenerateNext(fileSize - AdditionalFileSize), Times.Once);
            _fileWriterMock.Verify(x => x.Write("line\r\n"), Times.Once());
        }

        [Test]
        public void GeneratesFileWithBigSingleChunk()
        {
            const int bufferSize = 1024 * 1024 * 8;
            const int fileSize = bufferSize + 10;
            _chunkGeneratorMock.Setup(x => x.GenerateNext(fileSize - AdditionalFileSize)).Returns("line\r\n");
            var instance = CreateInstance();
            
            instance.Generate(fileSize);
            
            _chunkGeneratorMock.Verify(x => x.GenerateNext(fileSize - AdditionalFileSize), Times.Once);
            _fileWriterMock.Verify(x => x.Write("line\r\n"), Times.Once());
        }

        [Test]
        public void GeneratesFileWithTwoChunks()
        {
            const int bufferSize = 1024 * 1024 * 8;
            _chunkGeneratorMock
                .SetupSequence(x => x.GenerateNext(bufferSize))
                .Returns("line1\r\n")
                .Returns("line2\r\n");
            
            var instance = CreateInstance();
            
            instance.Generate(bufferSize * 2 + AdditionalFileSize);
            
            _chunkGeneratorMock.Verify(x => x.GenerateNext(bufferSize), Times.Exactly(2));
            _fileWriterMock.Verify(x => x.Write("line1\r\n"), Times.Once());
            _fileWriterMock.Verify(x => x.Write("line2\r\n"), Times.Once());
        }
    }
}