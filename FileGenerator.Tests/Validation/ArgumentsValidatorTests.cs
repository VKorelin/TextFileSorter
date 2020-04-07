using System;
using FileGenerator.IO;
using FileGenerator.Validation;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.Tests.Validation
{
    [TestFixture]
    public class ArgumentsValidatorTests
    {
        private Mock<IEncodingInfoProvider> _encodingInfoProviderMock;
        
        [SetUp]
        public void Setup()
        {
            _encodingInfoProviderMock = new Mock<IEncodingInfoProvider>();
            _encodingInfoProviderMock.Setup(x => x.GetStringLength(It.IsAny<long>())).Returns(13);
        }
        
        private ArgumentsValidator CreateInstance()
            => new ArgumentsValidator(_encodingInfoProviderMock.Object);

        [Test]
        public void ReturnsFalseIfArgsAreNull()
        {
            var instance = CreateInstance();

            var result = instance.IsValid(null, out _);
            
            result.ShouldBeFalse();
        }
        
        [Test]
        public void ReturnsFalseIfArgsAreEmpty()
        {
            var instance = CreateInstance();

            var result = instance.IsValid(Array.Empty<string>(), out _);
            
            result.ShouldBeFalse();
        }
        
        [Test]
        public void ReturnsFalseIfFirstArgumentIsNotLong()
        {
            var instance = CreateInstance();

            var result = instance.IsValid(new []{"c1"}, out _);
            
            result.ShouldBeFalse();
        }
        
        [TestCase(1)]
        [TestCase(11)]
        public void ReturnsFalseIfFileSizeIsLessThanDoubleMinimumEntryLength(int size)
        {
            _encodingInfoProviderMock.Setup(x => x.GetStringLength(size)).Returns(size);
            var instance = CreateInstance();

            var result = instance.IsValid(new []{size.ToString()}, out _);
            
            result.ShouldBeFalse();
            _encodingInfoProviderMock.Verify(x => x.GetStringLength(size), Times.Once);
        }
        
        [TestCase(12)]
        [TestCase(1333333)]
        public void ReturnsTrueIfFileSizeIsValid(int size)
        {
            _encodingInfoProviderMock.Setup(x => x.GetStringLength(size)).Returns(size);
            var instance = CreateInstance();

            var result = instance.IsValid(new []{size.ToString()}, out var actualSize);
            
            result.ShouldBeTrue();
            actualSize.ShouldBe(size);
        }
    }
}