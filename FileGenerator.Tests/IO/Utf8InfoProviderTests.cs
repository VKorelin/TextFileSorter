using System.Text;
using FileGenerator.IO;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.Tests.IO
{
    [TestFixture]
    public class Utf8InfoProviderTests
    {
        private static Utf8InfoProvider CreateInstance()
            => new Utf8InfoProvider();

        [Test]
        public void ReturnsUtf8AsEncoding()
        {
            var instance = CreateInstance();
            
            instance.CurrentEncoding.ShouldBe(Encoding.UTF8);
        }

        [Test]
        public void ReturnsTwoBytesAsAdditionalFileSize()
        {
            var instance = CreateInstance();
            
            instance.AdditionalFileSize.ShouldBe(3);
        }

        [Test]
        public void ReturnsTheSameNumberOfBytesForEachSymbolInString()
        {
            var instance = CreateInstance();

            var bytes = instance.GetBytesCountInStringLength(10);
            
            bytes.ShouldBe(10);
        }
        
        [Test]
        public void ReturnsTheSameNumberOfCharsForEachByte()
        {
            var instance = CreateInstance();

            var length = instance.GetStringLength(10);
            
            length.ShouldBe(10);
        }
    }
}