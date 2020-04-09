using System.Collections.Generic;
using System.IO;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    internal sealed class ChunkMergeService : IChunkMergeService
    {
        private const int RecordSize = 100;
        private const int MaxUsage = 500000000;
        private const double RecordOverhead = 7.5;

        private readonly IConfigurationProvider _configurationProvider;

        public ChunkMergeService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void MergeChunks(IList<string> chunkNames, string outputFileName)
        {
            var chunksCount = chunkNames.Count;

            // ReSharper disable once PossibleLossOfFraction
            var bufferLength = (int) (MaxUsage / chunksCount / RecordSize / RecordOverhead);

            var chunkReaders = new StreamReader[chunksCount];
            var chunkQueues = new Queue<string>[chunksCount];
            for (var i = 0; i < chunksCount; i++)
            {
                chunkReaders[i] = new StreamReader(chunkNames[i], _configurationProvider.Encoding);
                chunkQueues[i] = new Queue<string>(bufferLength);
                LoadQueue(chunkQueues[i], chunkReaders[i], bufferLength);
            }

            Merge(outputFileName, chunksCount, bufferLength, chunkQueues, chunkReaders);

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
            Queue<string>[] queues, 
            StreamReader[] readers)
        {
            using var writer = new StreamWriter(outputFileName, false, _configurationProvider.Encoding);

            while (true)
            {
                var lowestIndex = -1;
                var lowestEntry = "";
                for (var i = 0; i < chunksCount; i++)
                {
                    if (queues[i] != null)
                    {
                        string peek = queues[i].Peek();
                        if (lowestIndex < 0 || string.CompareOrdinal(peek, lowestEntry) < 0)
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

                writer.WriteLine(ReverseEntry(lowestEntry));

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

            writer.Close();
        }

        private static string ReverseEntry(string lowestValue)
        {
            var arr = lowestValue.Split(".");
            return $"{arr[2]}. {arr[0]}";
        }

        private static void LoadQueue(Queue<string> queue, TextReader file, int records)
        {
            for (var i = 0; i < records; i++)
            {
                if (file.Peek() < 0) 
                    break;
                queue.Enqueue(file.ReadLine());
            }
        }
    }
}