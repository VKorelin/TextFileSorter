using System;
using System.IO;
using NLog;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    internal sealed class ExternalMergeSorter : IExternalMergeSorter
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly Func<string, ISortedChunksService> _sortedChunksServiceFactory;
        private readonly IChunkMergeService _chunkMergeService;
        private readonly string _outputFileName;

        public ExternalMergeSorter(
            Func<string, ISortedChunksService> sortedChunksServiceFactory, 
            IChunkMergeService chunkMergeService, 
            IConfigurationProvider configurationProvider)
        {
            _sortedChunksServiceFactory = sortedChunksServiceFactory;
            _chunkMergeService = chunkMergeService;
            _outputFileName = configurationProvider.OutputFolder;
        }

        public bool Sort(string fileName)
        {
            try
            {
                Logger.Info($"Start sorting file {fileName}");
                
                Logger.Info($"Create sorted chunks");
                var sortedChunksService = _sortedChunksServiceFactory(fileName);
                var chunkNames = sortedChunksService.CreateSortedChunks();
                Logger.Info($"Chunks created and sorted");
            
                Logger.Info($"Merge sorted chunks");
                var outputFile = BuildOutputFileName(fileName);
                _chunkMergeService.MergeChunks(chunkNames, outputFile);
                Logger.Info($"Chunks merged to file: {outputFile}");
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Exception occured during file soring");
                return false;
            }
        }

        private string BuildOutputFileName(string sourceFileName)
        {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourceFileName);
            var ext = Path.GetExtension(sourceFileName);
            return Path.Combine(_outputFileName, $"{fileNameWithoutExt}_sorted.{ext}");
        }
    }
}