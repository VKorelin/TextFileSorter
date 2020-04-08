using System.Text;

namespace FileGenerator.IO
{
    /// <summary>
    /// Provides information about encoding that will be used in generated file
    /// </summary>
    public interface IEncodingInfoProvider
    {
        /// <summary>
        /// Encoding that will be used in generated file
        /// </summary>
        Encoding CurrentEncoding { get; }
        
        /// <summary>
        /// Additional bytes in each file
        /// </summary>
        int AdditionalFileSize { get; }

        /// <summary>
        /// Get size of string of particular length
        /// </summary>
        /// <param name="length">Length of string</param>
        /// <returns>Size of string</returns>
        long GetBytesCountInStringLength(long length);

        /// <summary>
        /// Get string length for particular bytes count
        /// </summary>
        /// <param name="bytesCount">Bytes for string</param>
        /// <returns>Length of string</returns>
        long GetStringLength(long bytesCount);
    }
}