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

        public void MergeChunks(IList<string> paths, string outputFileName)
        {
            int chunks = paths.Count; // Number of chunks
            int recordsize = 100; // estimated record size
            int records = 10000000; // estimated total # records
            int maxusage = 500000000; // max memory usage
            int buffersize = maxusage / chunks; // bytes of each queue
            double recordoverhead = 7.5; // The overhead of using Queue<>
            int bufferlen = (int) (buffersize / recordsize / recordoverhead); // number of records in each queue

            // Open the files
            StreamReader[] readers = new StreamReader[chunks];
            for (int i = 0; i < chunks; i++)
                readers[i] = new StreamReader(paths[i], _configurationProvider.Encoding);

            // Make the queues
            Queue<string>[] queues = new Queue<string>[chunks];
            for (int i = 0; i < chunks; i++)
                queues[i] = new Queue<string>(bufferlen);

            // Load the queues
            for (int i = 0; i < chunks; i++)
                LoadQueue(queues[i], readers[i], bufferlen);

            // Merge!
            StreamWriter sw = new StreamWriter(outputFileName, false, _configurationProvider.Encoding);
            int lowest_index, j, progress = 0;
            string lowest_value;

            while (true)
            {
                // Find the chunk with the lowest value
                lowest_index = -1;
                lowest_value = "";
                for (j = 0; j < chunks; j++)
                {
                    if (queues[j] != null)
                    {
                        if (lowest_index < 0 || string.CompareOrdinal(queues[j].Peek(), lowest_value) < 0)
                        {
                            lowest_index = j;
                            lowest_value = queues[j].Peek();
                        }
                    }
                }
                
                // Was nothing found in any queue? We must be done then.
                if (lowest_index == -1)
                {
                    break;
                }

                // Output it
                sw.WriteLine(ReverseEntry(lowest_value));

                // Remove from queue
                queues[lowest_index].Dequeue();
                // Have we emptied the queue? Top it up
                if (queues[lowest_index].Count == 0)
                {
                    LoadQueue(queues[lowest_index],
                        readers[lowest_index], bufferlen);
                    // Was there nothing left to read?
                    if (queues[lowest_index].Count == 0)
                    {
                        queues[lowest_index] = null;
                    }
                }
            }

            sw.Close();

            // Close and delete the files
            for (int i = 0; i < chunks; i++)
            {
                readers[i].Close();
                File.Delete(paths[i]);
            }
        }

        static void LoadQueue(Queue<string> queue, StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }

        private string ReverseEntry(string entry)
        {
            var arr = entry.Split(". ");
            return $"{arr[1]}. {arr[0]}";
        }
    }
}