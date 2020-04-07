using System.IO;

namespace FileGenerator.IO
{
    internal sealed class FileWriter : IFileWriter
    {
        private readonly StreamWriter _stream;
        
        public FileWriter(string path, IEncodingInfoProvider encodingProvider)
        {
            _stream = new StreamWriter(path, false, encodingProvider.CurrentEncoding);
        }
        
        public void WriteChunk(string chunk) => _stream.Write(chunk);

        public void Dispose() => _stream.Dispose();
    }
}