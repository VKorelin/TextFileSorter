using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    internal sealed class ChunkMergeService : IChunkMergeService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFilesMerger _filesMerger;

        private readonly ConcurrentQueue<string> _filesQueue;
        private readonly ConcurrentBag<string> _mergedFiles;

        private long _ramPerThread;

        public ChunkMergeService(IConfigurationProvider configurationProvider, IFilesMerger filesMerger)
        {
            _configurationProvider = configurationProvider;
            _filesMerger = filesMerger;
            _filesQueue = new ConcurrentQueue<string>();
            _mergedFiles = new ConcurrentBag<string>();
        }

        public void MergeChunks(IList<string> chunkNames, string outputFileName)
        {
            var chunksCount = Math.Min(chunkNames.Count, _configurationProvider.MexChunksNumberInMerge);

            var threadsCount = _configurationProvider.ThreadCount * chunksCount < chunkNames.Count
                ? _configurationProvider.ThreadCount
                : (chunkNames.Count + chunksCount - 1) / chunksCount;

            _ramPerThread = _configurationProvider.RamLimit / threadsCount;

            _filesQueue.Clear();
            foreach (var chunkName in chunkNames)
            {
                _filesQueue.Enqueue(chunkName);
            }

            _mergedFiles.Clear();

            var tasks = new Task[threadsCount];
            for (var i = 0; i < threadsCount; i++)
            {
                tasks[i] = Task.Run(() => DoMerge(chunksCount));
            }

            Task.WaitAll(tasks);

            if (_mergedFiles.Count == 1)
            {
                File.Move(_mergedFiles.First(), outputFileName, true);
                return;
            }

            MergeChunks(_mergedFiles.ToList(), outputFileName);
        }

        private void DoMerge(int chunksCount)
        {
            while (true)
            {
                var filesToMerge = new List<string>();
                for (var i = 0; i < chunksCount; i++)
                {
                    if (!_filesQueue.TryDequeue(out var file))
                    {
                        break;
                    }

                    filesToMerge.Add(file);
                }

                for (var i = 0; i < chunksCount; i++)
                {
                    if (!_filesQueue.TryDequeue(out var file))
                    {
                        break;
                    }

                    filesToMerge.Add(file);
                }

                if (!filesToMerge.Any())
                    return;

                if (filesToMerge.Count == 1)
                {
                    _mergedFiles.Add(filesToMerge.First());
                    return;
                }

                var mergedFile = _filesMerger.Merge(filesToMerge, _ramPerThread);
                _mergedFiles.Add(mergedFile);
            }
        }
    }
}