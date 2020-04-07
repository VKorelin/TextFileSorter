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
        
        private Mock<IChunkGenerator> _chunkGeneratorMock;
        private Mock<Func<string, IFileWriter>> _fileWriterFactoryMock;
        private Mock <IFileWriter> _fileWriterMock;
        private Mock<IFilePathProvider> _filePathProviderMock;
        private Mock<IEncodingInfoProvider> _encodingInfoProviderMock;

        [SetUp]
        public void Setup()
        {
            _chunkGeneratorMock = new Mock<IChunkGenerator>();
            _chunkGeneratorMock.Setup(x => x.GenerateNext(It.IsAny<long>())).Returns(string.Empty);
            
            _encodingInfoProviderMock = new Mock<IEncodingInfoProvider>();
            _encodingInfoProviderMock.Setup(x => x.GetBytesCountInStringLength(2)).Returns(2);

            _filePathProviderMock = new Mock<IFilePathProvider>();
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
        public void GeneratesFileWithSmallSingleTruncatedFileChunk()
        {
            _chunkGeneratorMock.Setup(x => x.GenerateNext(1026)).Returns("line\r\n");
            var instance = CreateInstance();
            
            instance.Generate(1024);
            
            _chunkGeneratorMock.Verify(x => x.GenerateNext(1026), Times.Once);
            _fileWriterMock.Verify(x => x.Write("line"), Times.Once());
        }
        
        [Test]
        public void GeneratesFileWithSmallSingleChunkWithoutTruncating()
        {
            _chunkGeneratorMock.Setup(x => x.GenerateNext(1026)).Returns("line");
            var instance = CreateInstance();
            
            instance.Generate(1024);
            
            _chunkGeneratorMock.Verify(x => x.GenerateNext(1026), Times.Once);
            _fileWriterMock.Verify(x => x.Write("line"), Times.Once());
        }

        [Test]
        public void GeneratesFileWithBigSingleChunkWithoutTruncating()
        {
            var bufferSize = 1024 * 1024 * 8;
            _chunkGeneratorMock.Setup(x => x.GenerateNext(bufferSize + 12)).Returns("line");
            var instance = CreateInstance();
            
            instance.Generate(bufferSize + 10);
            
            _chunkGeneratorMock.Verify(x => x.GenerateNext(bufferSize + 12), Times.Once);
            _fileWriterMock.Verify(x => x.Write("line"), Times.Once());
        }
        
        [Test]
        public void GeneratesFileWithBigSingleTruncatedFileChunk()
        {
            var bufferSize = 1024 * 1024 * 8;
            _chunkGeneratorMock.Setup(x => x.GenerateNext(bufferSize + 12)).Returns("line\r\n");
            var instance = CreateInstance();
            
            instance.Generate(bufferSize + 10);
            
            _chunkGeneratorMock.Verify(x => x.GenerateNext(bufferSize + 12), Times.Once);
            _fileWriterMock.Verify(x => x.Write("line"), Times.Once());
        }

        [Test]
        public void GeneratesFileWithTwoChunksWithTruncating()
        {
            var bufferSize = 1024 * 1024 * 8;
            _chunkGeneratorMock.Setup(x => x.GenerateNext(bufferSize)).Returns("line1\r\n");
            _chunkGeneratorMock.Setup(x => x.GenerateNext(bufferSize + 2)).Returns("line2\r\n");
            
            var instance = CreateInstance();
            
            instance.Generate(bufferSize * 2);
            
            _chunkGeneratorMock.Verify(x => x.GenerateNext(bufferSize), Times.Once);
            _chunkGeneratorMock.Verify(x => x.GenerateNext(bufferSize + 2), Times.Once);
            _fileWriterMock.Verify(x => x.Write("line1\r\n"), Times.Once());
            _fileWriterMock.Verify(x => x.Write("line2"), Times.Once());
        }
    }
}