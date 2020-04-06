﻿using System;
using FileGenerator.IO;

namespace FileGenerator.Generation
{
    public class Generator : IGenerator
    {
        private const int DefaultBufferSize = 1024 * 1024 * 8;

        private readonly IChunkGenerator _chunkGenerator;
        private readonly IEncodingInfoProvider _encodingInfoProvider;
        private readonly Func<string, IFileWriter> _fileWrapperFactory;
        private readonly IFilePathProvider _filePathProvider;

        public Generator(
            IChunkGenerator chunkGenerator, 
            IEncodingInfoProvider encodingInfoProvider, 
            Func<string, IFileWriter> fileWrapperFactory,
            IFilePathProvider filePathProvider)
        {
            _chunkGenerator = chunkGenerator;
            _fileWrapperFactory = fileWrapperFactory;
            _filePathProvider = filePathProvider;
            _encodingInfoProvider = encodingInfoProvider;
        }

        public void Generate(long fileSize)
        {
            long currentFileSize = 0;

            using (var fileWriter = _fileWrapperFactory.Invoke(_filePathProvider.GetPath()))
            {
                var canGenerate = true;
                while (canGenerate)
                {
                    var bufferSize = CalculateBufferSize(fileSize - currentFileSize, ref canGenerate);
                    var chunk = _chunkGenerator.GenerateNext(bufferSize);
                    fileWriter.WriteChunk(chunk);
                    currentFileSize += _encodingInfoProvider.GetBytesCount(chunk);
                }
            }
        }

        private static long CalculateBufferSize(long freeSpace, ref bool canGenerate)
        {
            if (DefaultBufferSize < freeSpace - DefaultBufferSize)
            {
                return DefaultBufferSize;
            }

            canGenerate = false;
            return freeSpace;
        }
    }
}