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
            var threadsCount = _configurationProvider.ThreadCount * 2 < chunkNames.Count
                ? _configurationProvider.ThreadCount
                : (chunkNames.Count + 1) / 2;

            _ramPerThread = _configurationProvider.RamLimit / threadsCount;
                
            _filesQueue = new ConcurrentQueue<string>(chunkNames);
            _mergedFiles.Clear();

            var tasks = new Task[threadsCount];
            for (var i = 0; i < threadsCount; i++)
            {
                tasks[i] = Task.Run(DoTask);
            }

            Task.WaitAll(tasks);

            if (_mergedFiles.Count == 1)
            {
                File.Move(_mergedFiles.First(), outputFileName, true);
                return;
            }
            
            MergeChunks(_mergedFiles.ToList(), outputFileName);
        }

        private void DoTask()
        {
            while (true)
            {
                if (!_filesQueue.TryDequeue(out var file1))
                {
                    return;
                }

                if (!_filesQueue.TryDequeue(out var file2))
                {
                    _mergedFiles.Add(file1);
                    return;
                }

                var mergedFile = MergeTwoFiles(file1, file2);
                _mergedFiles.Add(mergedFile);
            }
        }

        private string MergeTwoFiles(string file1, string file2)
        {
            var bytesPerFile = _ramPerThread / 2;
            var bufferLength = (int) (bytesPerFile / MaxEntryLength);

            var writeBufferLength = _configurationProvider.Encoding.GetMaxCharCount((int) bytesPerFile);

            var fileReader1 = new StreamReader(file1, _configurationProvider.Encoding);
            var fileReader2 = new StreamReader(file2, _configurationProvider.Encoding);

            var fileQueue1 = new Queue<Entry>(bufferLength);
            var fileQueue2 = new Queue<Entry>(bufferLength);

            LoadQueue(fileQueue1, fileReader1, bufferLength);
            LoadQueue(fileQueue2, fileReader2, bufferLength);

            var mergedFile = BuildOutputName(file1);
            
            Merge(mergedFile, 2, bufferLength, new [] { fileQueue1, fileQueue2 }, new [] { fileReader1, fileReader2 }, writeBufferLength);

            fileReader1.Close();
            fileReader2.Close();

            File.Delete(file1);
            File.Delete(file2);

            return mergedFile;
        }
        
        private string BuildOutputName(string sourceFile)
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