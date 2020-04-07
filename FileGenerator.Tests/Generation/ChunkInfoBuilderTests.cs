using FileGenerator.Domain;
using FileGenerator.Generation;
using FileGenerator.IO;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.Tests.Generation
{
    [TestFixture]
    public class ChunkInfoBuilderTests
    {
        private Mock<IRandomNumberGenerator> _randomNumberGeneratorMock;
        private Mock<IEncodingInfoProvider> _encodingInfoProviderMock;

        [SetUp]
        public void Setup()
        {
            _randomNumberGeneratorMock = new Mock<IRandomNumberGenerator>();
            _randomNumberGeneratorMock
                .Setup(x => x.Generate(EntryInfo.MinLength, EntryInfo.MaxEntryLength))
                .Returns(EntryInfo.MinLength);
            
            _encodingInfoProviderMock = new Mock<IEncodingInfoProvider>();
            _encodingInfoProviderMock.Setup(x => x.GetStringLength(20)).Returns(20);
        }
        
        private ChunkInfoBuilder CreateInstance()
            => new ChunkInfoBuilder(_randomNumberGeneratorMock.Object, _encodingInfoProviderMock.Object);

        [Test]
        public void GetsStringLengthFromBytesCount()
        {
            var instance = CreateInstance();

            instance.Build(20);
            
            _encodingInfoProviderMock.Verify(x => x.GetStringLength(20), Times.Once);
        }

        [Test]
        public void GetsOnlyEntryAndRepeatedEntry()
        {
            _randomNumberGeneratorMock
                .Setup(x => x.Generate(EntryInfo.MinLength, EntryInfo.MaxEntryLength))
                .Returns(11);
            
            var instance = CreateInstance();

            var result = instance.Build(20);

            result.EntryInfos.ShouldBeEmpty();
            result.RepeatedEntry.LineLength.ShouldBe(3);
            result.RepeatedEntry.NumberLength.ShouldBe(3);
        }
        
        [Test]
        public void GetsChunkInfoWithEntries()
        {
            _randomNumberGeneratorMock
                .Setup(x => x.Generate(EntryInfo.MinLength, EntryInfo.MaxEntryLength))
                .Returns(EntryInfo.MinLength);
            
            var instance = CreateInstance();

            var result = instance.Build(20);
            
            var entry = result.EntryInfos.ShouldHaveSingleItem();
            entry.LineLength.ShouldBe(2);
            entry.NumberLength.ShouldBe(2);
            
            result.RepeatedEntry.LineLength.ShouldBe(1);
            result.RepeatedEntry.NumberLength.ShouldBe(1);
        }
    }
}