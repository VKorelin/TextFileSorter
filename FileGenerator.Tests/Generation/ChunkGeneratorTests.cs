using System.Collections.Generic;
using FileGenerator.Domain;
using FileGenerator.Generation;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.Tests.Generation
{
    [TestFixture]
    public class ChunkGeneratorTests
    {
        private Mock<IRandomNumberGenerator> _randomNumberGeneratorMock;
        private Mock<IRandomStringGenerator> _randomStringGeneratorMock;
        private Mock<IChunkInfoBuilder> _chunkInfoBuilderMock;

        private ChunkInfo _chunkInfo;

        [SetUp]
        public void Setup()
        {
            var entriesInfo = new List<EntryInfo>
            {
                new EntryInfo(2, 22),
                new EntryInfo(3, 33)
            };
            _chunkInfo = new ChunkInfo(entriesInfo, new EntryInfo(1, 11));
            
            _chunkInfoBuilderMock = new Mock<IChunkInfoBuilder>();
            _chunkInfoBuilderMock.Setup(x => x.Build(10)).Returns(_chunkInfo);
            
            _randomStringGeneratorMock = new Mock<IRandomStringGenerator>();
            _randomStringGeneratorMock.Setup(x => x.Generate(11)).Returns("line1");
            _randomStringGeneratorMock.Setup(x => x.Generate(22)).Returns("line2");
            _randomStringGeneratorMock.Setup(x => x.Generate(33)).Returns("line3");
            
            _randomNumberGeneratorMock = new Mock<IRandomNumberGenerator>();
            _randomNumberGeneratorMock
                .SetupSequence(x => x.Generate(1, 10))
                .Returns(1)
                .Returns(4);
            _randomNumberGeneratorMock.Setup(x => x.Generate(10, 100)).Returns(2);
            _randomNumberGeneratorMock.Setup(x => x.Generate(100, 1000)).Returns(3);
        }

        private ChunkGenerator CreateInstance()
            => new ChunkGenerator(
                _randomNumberGeneratorMock.Object,
                _randomStringGeneratorMock.Object,
                _chunkInfoBuilderMock.Object);

        [Test]
        public void GeneratesChunk()
        {
            var instance = CreateInstance();

            var chunk = instance.GenerateNext(10);
            
            chunk.ShouldBe("1. line1\r\n2. line2\r\n3. line3\r\n4. line1\r\n");
        }
    }
}