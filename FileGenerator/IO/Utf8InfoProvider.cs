﻿using System.Text;

namespace FileGenerator.IO
{
    /// <summary>
    /// Provides information about working with UTF-8
    /// </summary>
    internal sealed class Utf8InfoProvider : IEncodingInfoProvider
    {
        /// <summary>
        /// UTF8 encoding
        /// </summary>
        public Encoding CurrentEncoding => Encoding.UTF8;

        /// <summary>
        /// Additional bytes in each file of UTF8
        /// </summary>
        public int AdditionalFileSize => 3;
        
        ///<inheritdoc/>
        public long GetBytesCountInStringLength(long length) => length;

        ///<inheritdoc/>
        public long GetStringLength(long bytesCount) => bytesCount;
    }
}