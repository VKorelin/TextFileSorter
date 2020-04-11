using FileGenerator.Configuration;
using FileGenerator.Generation;
using FileGenerator.IO;
using Moq;
using NUnit.Framework;

namespace FileGenerator.Tests.Generation
{
    [TestFixture]
    public class GeneratorTests
    {
        private const int AdditionalFileSize = 1;
        
        private Mock<IChunkGenerationJob> _chunkGenerationJobMock;
        private Mock<IEncodingInfoProvider> _encodingInfoProviderMock;
        private Mock<IConfigurationProvider> _configurationProviderMock;

        [SetUp]
        public void Setup()
        {
            _chunkGenerationJobMock = new Mock<IChunkGenerationJob>();
            
            _encodingInfoProviderMock = new Mock<IEncodingInfoProvider>();
            _encodingInfoProviderMock.Setup(x => x.GetBytesCountInStringLength(2)).Returns(2);
            _encodingInfoProviderMock.SetupGet(x => x.AdditionalFileSize).Returns(1);
            
            _configurationProviderMock = new Mock<IConfigurationProvider>();
            _configurationProviderMock.SetupGet(x => x.DefaultBufferSize).Returns(1024 * 1024 * 8);
        }
        
        private Generator CreateInstance()
            => new Generator(
                _chunkGenerationJobMock.Object,
                _encodingInfoProviderMock.Object,
                _configurationProviderMock.Object);

        [Test]
        public void GeneratesFileWithSmallSingleChunk()
        {
            const int fileSize = 1024;
            var instance = CreateInstance();
            
            instance.Generate(fileSize);
            
            _chunkGenerationJobMock.Verify(x => x.Start(), Times.Once);
            _chunkGenerationJobMock.Verify(x => x.AddNext(fileSize - AdditionalFileSize), Times.Once);
            _chunkGenerationJobMock.Verify(x => x.Stop(), Times.Once);
        }

        [Test]
        public void GeneratesFileWithBigSingleChunk()
        {
            const int bufferSize = 1024 * 1024 * 8;
            const int fileSize = bufferSize + 10;
            var instance = CreateInstance();
            
            instance.Generate(fileSize);
            
            _chunkGenerationJobMock.Verify(x => x.Start(), Times.Once);
            _chunkGenerationJobMock.Verify(x => x.AddNext(fileSize - AdditionalFileSize), Times.Once);
            _chunkGenerationJobMock.Verify(x => x.Stop(), Times.Once);
        }

        [Test]
        public void GeneratesFileWithTwoChunks()
        {
            const int bufferSize = 1024 * 1024 * 8;
            
            var instance = CreateInstance();
            
            instance.Generate(bufferSize * 2 + AdditionalFileSize);
            
            _chunkGenerationJobMock.Verify(x => x.Start(), Times.Once);
            _chunkGenerationJobMock.Verify(x => x.AddNext(bufferSize), Times.Exactly(2));
            _chunkGenerationJobMock.Verify(x => x.Stop(), Times.Once);
        }
    }
}