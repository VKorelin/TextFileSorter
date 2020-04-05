using System.IO;

namespace FileGenerator.IO
{
    internal sealed class FileWriter : IFileWriter
    {
        private readonly StreamWriter _stream;
        
        public FileWriter(IEncodingProvider encodingProvider)
        {
            _stream = new StreamWriter("data.txt", false, encodingProvider.Encoding);
        }
        
        public void WriteChunk(string chunk) => _stream.WriteLine(chunk);

        public void Dispose() => _stream.Dispose();
    }
}