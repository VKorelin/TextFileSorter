using System.Collections.Generic;
using System.IO;
using System.Text;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    internal sealed class ChunkMergeService : IChunkMergeService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ChunkMergeService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void MergeChunks(IList<string> chunkNames, string outputFileName)
        {
            var chunksCount = chunkNames.Count;

            var bytesPerChunk = _configurationProvider.RamLimit / (chunksCount + 1);
            const int maxEntryLength = 100;
            var bufferLength = (int) (bytesPerChunk / maxEntryLength);

            var writeBufferLength = _configurationProvider.Encoding.GetMaxCharCount((int) bytesPerChunk);

            var chunkReaders = new StreamReader[chunksCount];
            var chunkQueues = new Queue<Entry>[chunksCount];
            for (var i = 0; i < chunksCount; i++)
            {
                chunkReaders[i] = new StreamReader(chunkNames[i], _configurationProvider.Encoding);
                chunkQueues[i] = new Queue<Entry>(bufferLength);
                LoadQueue(chunkQueues[i], chunkReaders[i], bufferLength);
            }

            Merge(outputFileName, chunksCount, bufferLength, chunkQueues, chunkReaders, writeBufferLength);

            for (var i = 0; i < chunksCount; i++)
            {
                chunkReaders[i].Close();
                File.Delete(chunkNames[i]);
            }
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