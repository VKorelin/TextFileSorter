using System;
using System.Collections.Generic;
using FileGenerator.Generation;
using FileGenerator.IO;
using FileGenerator.Validation;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.Tests
{
    public class BootstrapperTests
    {
        private static readonly string[] Args = {"512"};
        private static readonly List<string[]> InvalidArgs = new List<string[]>
        {
            Array.Empty<string>(),
            new [] {"f3"},
            new [] {"-1"},
            new [] {"f3", "12"},
            new [] {(string)null},
            null
        };

        private Mock<IArgumentsValidator> _argumentsValidatorMock;
        private Mock<IGenerator> _generatorMock;
        private Mock<IEncodingInfoProvider> _encodingInfoProviderMock;
        
        private long _fileSize = 512;

        [SetUp]
        public void Setup()
        {
            _argumentsValidatorMock = new Mock<IArgumentsValidator>();
            _argumentsValidatorMock
                .Setup(x => x.IsValid(Args, out _fileSize))
                .Returns(true);
            
            _encodingInfoProviderMock = new Mock<IEncodingInfoProvider>();
            
            _generatorMock = new Mock<IGenerator>();
        }

        private Bootstrapper CreateInstance()
            => new Bootstrapper(
                _argumentsValidatorMock.Object,
                _generatorMock.Object,
                _encodingInfoProviderMock.Object);

        [Test]
        public void ValidatesArguments()
        {
            var instance = CreateInstance();

            instance.Start(Args);
            
            _argumentsValidatorMock.Verify(x => x.IsValid(Args, out _fileSize), Times.Once);
        }

        [TestCaseSource(nameof(InvalidArgs))]
        public void ReturnsArgumentsInvalidIfFileSizeCannotBeParsed(string[] args)
        {
            var instance = CreateInstance();

            var result = instance.Start(args);
            
            result.ShouldBe(GenerationResult.ArgumentsInvalid);
            _encodingInfoProviderMock.Verify(x => x.GetBytesCountInStringLength(12), Times.Once);
        }

        [Test]
        public void GeneratesFile()
        {
            var instance = CreateInstance();

            instance.Start(Args);
            
            _generatorMock.Verify(x => x.Generate(_fileSize), Times.Once);
        }
        
        [Test]
        public void ReturnSuccessIfFileGenerated()
        {
            var instance = CreateInstance();

            var result = instance.Start(Args);
            
            result.ShouldBe(GenerationResult.Success);
        }
        
        [Test]
        public void ReturnGenerationFailedIfErrorOccuredDuringGeneration()
        {
            _generatorMock.Setup(x => x.Generate(It.IsAny<long>())).Throws<Exception>();
            var instance = CreateInstance();

            var result = instance.Start(Args);
            
            result.ShouldBe(GenerationResult.GenerationError);
        }
    }
}