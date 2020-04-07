using System.Text;
using FileGenerator.IO;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.Tests.IO
{
    [TestFixture]
    public class UnicodeInfoProviderTests
    {
        private static UnicodeInfoProvider CreateInstance()
            => new UnicodeInfoProvider();

        [Test]
        public void ReturnsUnicodeAsEncoding()
        {
            var instance = CreateInstance();
            
            instance.CurrentEncoding.ShouldBe(Encoding.Unicode);
        }

        [Test]
        public void ReturnsTheDoubleNumberOfBytesForEachSymbolInString()
        {
            var instance = CreateInstance();

            var bytes = instance.GetBytesCountInStringLength(10);
            
            bytes.ShouldBe(20);
        }
        
        [Test]
        public void ReturnsHalfNumberOfCharsForEachByte()
        {
            var instance = CreateInstance();

            var length = instance.GetStringLength(10);
            
            length.ShouldBe(5);
        }
    }
}