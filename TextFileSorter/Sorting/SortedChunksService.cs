using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    public class SortedChunksService : ISortedChunksService
    {
        private readonly Func<string, IChunkFileReader> _chunkFileReaderFactory;
        
        private readonly string _fileName;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly string _fileNameWithoutExtension;
        public readonly string _chunksFolder;
        
        private readonly BlockingCollection<IList<string>> _chunksQueue;
        private readonly ConcurrentBag<string> _chunkNames;
        private readonly Task[] _sortTasks;

        private int _chunkNumber;

        public SortedChunksService(
            string fileName,
            IConfigurationProvider configurationProvider,
            Func<string, IChunkFileReader> chunkFileReaderFactory)
        {
            _fileName = fileName;
            _configurationProvider = configurationProvider;
            _fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_fileName);
            _chunkFileReaderFactory = chunkFileReaderFactory;
            
            _chunksFolder = Path.Combine(configurationProvider.OutputFolder, "chunks");
            if (!Directory.Exists(_chunksFolder))
            {
                Directory.CreateDirectory(_chunksFolder);
            }
            
            _chunkNames = new ConcurrentBag<string>();
            _chunksQueue = new BlockingCollection<IList<string>>(new ConcurrentQueue<IList<string>>(), configurationProvider.ThreadCount - 1);
            
            _sortTasks = new Task[configurationProvider.ThreadCount - 1];
            for (var i = 0; i < _sortTasks.Length; i++)
            {
                _sortTasks[i] = Task.Run(SortNext);
            }
        }

        public IList<string> CreateSortedChunks()
        {
            using (var reader = _chunkFileReaderFactory(_fileName))
            {
                ReadChunkResult nextChunk;
                do
                {
                    nextChunk = reader.ReadNextChunk();
                    _chunksQueue.Add(nextChunk.Chunk);
                } while (!nextChunk.IsLastChunk);
            }

            _chunksQueue.CompleteAdding();
            Task.WaitAll(_sortTasks);
            return _chunkNames.ToArray();
        }

        private void SortNext()
        {
            while (!_chunksQueue.IsCompleted || _chunksQueue.Count > 0)
            {
                try
                {
                    var entries = _chunksQueue.Take()
                        .Select(Entry.Build)
                        .OrderBy(x => x);
            
                    var chunkIdx = Interlocked.Increment(ref _chunkNumber);
            
                    var savePath = Path.Combine(_chunksFolder, $"{_fileNameWithoutExtension}_{chunkIdx}.txt");
                    File.WriteAllLines(savePath, entries.Select(x => x.ToString()), _configurationProvider.Encoding);
                    _chunkNames.Add(savePath);
                }
                catch (InvalidOperationException)
                {
                    // An InvalidOperationException means that Take() was called on a completed collection
                    break;
                }
            }
        }
    }
}