using System;
using FileGenerator.Generation;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.Tests.Generation
{
    [TestFixture]
    public class RandomStringGeneratorTests
    {
        private Mock<IRandomNumberGenerator> _randomNumberGeneratorMock;

        [SetUp]
        public void Setup()
        {
            _randomNumberGeneratorMock = new Mock<IRandomNumberGenerator>();
        }
        
        private RandomStringGenerator CreateInstance()
            => new RandomStringGenerator(_randomNumberGeneratorMock.Object);

        [TestCase(0)]
        [TestCase(-1)]
        public void ThrowsExceptionIfSizeIsLessThanOne(long size)
        {
            var instance = CreateInstance();

            var ex = Should.Throw<ArgumentException>(() => instance.Generate(size));
            ex.Message.ShouldBe("'size' could not be less than zero");
        }

        [Test]
        public void GeneratesRandomBytesArrayOfParticularSize()
        {
            var instance = CreateInstance();

            instance.Generate(10);
            
            _randomNumberGeneratorMock.Verify(x => x.GenerateNextBytes(10), Times.Once);
        }
        
        [Test]
        public void GeneratesRandomString()
        {
            _randomNumberGeneratorMock.Setup(x => x.GenerateNextBytes(3)).Returns(new byte[] {126, 1, 2});
            var instance = CreateInstance();

            var result = instance.Generate(3);
            
            result.ShouldBe("ABC");
        }
    }
}