using System.IO;

namespace FileGenerator.IO
{
    ///<inheritdoc/>
    internal sealed class FileWriter : IFileWriter
    {
        private readonly StreamWriter _stream;
        
        /// <summary>
        /// Opens stream to generated file. If file with the same name exists, replace it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encodingProvider"></param>
        public FileWriter(string path, IEncodingInfoProvider encodingProvider)
        {
            _stream = new StreamWriter(path, false, encodingProvider.CurrentEncoding);
        }
        
        ///<inheritdoc/>
        public void Write(string data) => _stream.Write(data);

        /// <summary>
        /// Disposes stream
        /// </summary>
        public void Dispose() => _stream.Dispose();
    }
}