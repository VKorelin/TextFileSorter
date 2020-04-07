using System.IO;
using FileGenerator.IO;
using NUnit.Framework;
using Shouldly;

namespace FileGenerator.Tests.IO
{
    [TestFixture]
    public class FilePathProviderTests
    {
        private static FilePathProvider CreateInstance()
            => new FilePathProvider();

        [Test]
        public void GetsFileNameWithRelativePath()
        {
            var instance = CreateInstance();

            var path = instance.GetPath();

            var dir = Path.GetDirectoryName(path);
            dir.ShouldBe("files");
        }
        
        [Test]
        public void GetsFileNameWithCorrectEnding()
        {
            var instance = CreateInstance();

            var path = instance.GetPath();

            var file = Path.GetFileName(path);
            file.EndsWith("-data.txt").ShouldBeTrue();
        }
    }
}