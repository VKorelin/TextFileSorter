using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    internal sealed class ChunkMergeService : IChunkMergeService
    {
        private const int MaxEntryLength = 100;

        private readonly IConfigurationProvider _configurationProvider;

        private ConcurrentQueue<string> _filesQueue;
        private readonly ConcurrentBag<string> _mergedFiles;

        private long _ramPerThread;

        public ChunkMergeService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
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
                
            _filesQueue = new ConcurrentQueue<string>(chunkNames);
            _mergedFiles.Clear();

            var tasks = new Task[threadsCount];
            for (var i = 0; i < threadsCount; i++)
            {
                tasks[i] = Task.Run(() => DoTask(chunksCount));
            }

            Task.WaitAll(tasks);

            if (_mergedFiles.Count == 1)
            {
                File.Move(_mergedFiles.First(), outputFileName, true);
                return;
            }
            
            MergeChunks(_mergedFiles.ToList(), outputFileName);
        }

        private void DoTask(int chunksCount)
        {
            while (true)
            {
                var filesToMerge = new List<string>();
                for (int i = 0; i < chunksCount; i++)
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

                var mergedFile = MergeTwoFiles(filesToMerge);
                _mergedFiles.Add(mergedFile);
            }
        }

        private string MergeTwoFiles(IList<string> filesToMerge)
        {
            var bytesPerFile = _ramPerThread / filesToMerge.Count;
            var bufferLength = (int) (bytesPerFile / MaxEntryLength);

            var writeBufferLength = _configurationProvider.Encoding.GetMaxCharCount((int) bytesPerFile);

            var fileReaders = new StreamReader[filesToMerge.Count];
            var entryQueues = new Queue<Entry>[filesToMerge.Count];
            for (var i = 0; i < filesToMerge.Count; i++)
            {
                fileReaders[i] = new StreamReader(filesToMerge[i], _configurationProvider.Encoding);
                entryQueues[i] = new Queue<Entry>(bufferLength);
                LoadQueue(entryQueues[i], fileReaders[i], bufferLength);
            }

            var mergedFile = BuildOutputName(filesToMerge.First());
            
            Merge(mergedFile, filesToMerge.Count, bufferLength, entryQueues, fileReaders, writeBufferLength);

            for (var i = 0; i < filesToMerge.Count; i++)
            {
                fileReaders[i].Close();
                File.Delete(filesToMerge[i]);
            }

            return mergedFile;
        }
        
        private static string BuildOutputName(string sourceFile)
        {
            var dir = Path.GetDirectoryName(sourceFile);
            return Path.Combine(dir, Guid.NewGuid() + ".txt");
        }

        private void Merge(
            string outputFileName,
            int chunksCount,
            int bufferLength,
            Queue<Entry>[] queues,
            StreamReader[] readers,
            int writeBufferLength)
        {
            using var writer = new StreamWriter(outputFileName, false, _configurationProvider.Encoding);
            var buffer = new StringBuilder();

            while (true)
            {
                var lowestIndex = -1;
                var lowestEntry = default(Entry);
                for (var i = 0; i < chunksCount; i++)
                {
                    if (queues[i] != null)
                    {
                        if (lowestIndex < 0 || queues[i].Peek().CompareTo(lowestEntry) < 0)
                        {
                            lowestIndex = i;
                            lowestEntry = queues[i].Peek();
                        }
                    }
                }
                
                if (lowestIndex == -1)
                {
                    break;
                }
                
                buffer.AppendLine(lowestEntry.ToString());
                if (buffer.Length > writeBufferLength)
                {
                    writer.Write(buffer);
                    buffer.Clear();
                }

                queues[lowestIndex].Dequeue();
                if (queues[lowestIndex].Count == 0)
                {
                    LoadQueue(queues[lowestIndex], readers[lowestIndex], bufferLength);
                    if (queues[lowestIndex].Count == 0)
                    {
                        queues[lowestIndex] = null;
                    }
                }
            }

            writer.Write(buffer);
            writer.Close();
        }

        private static void LoadQueue(Queue<Entry> queue, TextReader file, int records)
        {
            for (var i = 0; i < records; i++)
            {
                if (file.Peek() < 0) 
                    break;
                queue.Enqueue(Entry.Build(file.ReadLine()));
            }
        }
    }
}