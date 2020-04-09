using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    public class SortedChunksService : ISortedChunksService
    {
        private static readonly char[] SplitChars = {'\r', '\n'};
        
        private readonly Func<string, IChunkFileReader> _chunkFileReaderFactory;
        private readonly IEncodingInfoProvider _encodingInfoProvider;
        private readonly string _fileName;
        private readonly string _fileNameWithoutExtension;
        private readonly Encoding _encoding;
        private readonly string _outputFolder;
        
        private readonly BlockingCollection<string[]> _chunksQueue;
        private readonly ConcurrentBag<string> _chunkNames;
        private readonly Task[] _sortTasks;

        private int _chunkNumber;

        public SortedChunksService(
            string fileName,
            IConfigurationProvider configurationProvider,
            IEncodingInfoProvider encodingInfoProvider,
            Func<string, IChunkFileReader> chunkFileReaderFactory)
        {
            _fileName = fileName;
            _encodingInfoProvider = encodingInfoProvider;
            _fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_fileName);
            _chunkFileReaderFactory = chunkFileReaderFactory;
            _encoding = configurationProvider.Encoding;
            _outputFolder = configurationProvider.OutputFolder;
            
            _chunkNames = new ConcurrentBag<string>();
            _chunksQueue = new BlockingCollection<string[]>(new ConcurrentQueue<string[]>(), configurationProvider.ThreadCount);
            
            _sortTasks = new Task[configurationProvider.ThreadCount];
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
                var shift = 0;
                do
                {
                    nextChunk = reader.ReadNextChunk(shift);
                    var chunkString = _encodingInfoProvider.GetString(nextChunk.Chunk);

                    IEnumerable<string> lines = chunkString.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
                    if (!nextChunk.IsLastChunk && lines.Any())
                    {
                        shift = - lines.Last().Length * 2;
                        lines = lines.SkipLast(1);
                    }

                    _chunksQueue.Add(lines.ToArray());
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
                var lines = _chunksQueue.Take();
                lines = lines
                    .Select(x => x.Split(". "))
                    .OrderBy(x => x[1])
                    .ThenBy(x => x[0])
                    .Select(x => $"{x[0]}. {x[1]}")
                    .ToArray();
            
                var chunkIdx = Interlocked.Increment(ref _chunkNumber);
            
                var savePath = Path.Combine(_outputFolder, "chunks", $"{_fileNameWithoutExtension}_{chunkIdx}.txt");
                File.WriteAllLines(savePath, lines, _encoding);
                _chunkNames.Add(savePath);
            }
        }
    }
}