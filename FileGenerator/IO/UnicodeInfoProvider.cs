using System.Text;

namespace FileGenerator.IO
{
    /// <summary>
    /// Provides information about working with Unicode
    /// </summary>
    internal sealed class UnicodeInfoProvider : IEncodingInfoProvider
    {
        /// <summary>
        /// Unicode encoding
        /// </summary>
        public Encoding CurrentEncoding => Encoding.Unicode;

        ///<inheritdoc/>
        public long GetBytesCountInStringLength(long length) => length * 2;

        ///<inheritdoc/>
        public long GetStringLength(long bytesCount) => bytesCount / 2;
    }
}