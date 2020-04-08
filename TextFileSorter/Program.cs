using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TextFileSorter
{
    class Program
    {
        public const string ChunksFolder = @"C:\Users\VKorelin\source\repos\TextFileSorter\TestData\chunks";
        public const string SourceFolder = @"C:\Users\VKorelin\source\repos\TextFileSorter\TestData";

        static void Main(string[] args)
        {
            if (!Directory.Exists(ChunksFolder))
            {
                Directory.CreateDirectory(ChunksFolder);
            }
            
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var fileName = Path.Combine(SourceFolder, "data_10Gb.txt");
            Console.WriteLine($"Start {DateTime.Now}");
            
            var sorter = new Sorter();
            sorter.CreateSortedChunks(fileName, Encoding.Unicode);
            Console.WriteLine($"Chunks created and sorted {DateTime.Now}");
            
            MergeTheChunks(fileName);
            Console.WriteLine($"Chunks merged {DateTime.Now}");
            
            stopwatch.Stop();
            Console.WriteLine($"Done. Finished in {stopwatch.Elapsed.ToString()}");

            Console.ReadKey();
        }

        private static StreamWriter CreateStreamWriter(string fileName) 
            => new StreamWriter(fileName, false, Encoding.Unicode);
        
        private static void MergeTheChunks(string fileName)
        {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            
            var paths = Directory.GetFiles(ChunksFolder, $"{fileNameWithoutExt}*.txt");
            var chunks = paths.Length; // Number of chunks
            var recordsize = 100; // estimated record size
            var records = 10000000; // estimated total # records
            var maxusage = 500000000; // max memory usage
            var buffersize = maxusage / chunks; // bytes of each queue
            var recordoverhead = 7.5; // The overhead of using Queue<>
            var bufferlen = (int) (buffersize / recordsize / recordoverhead); // number of records in each queue

            // Open the files and make queues
            var readers = new StreamReader[chunks];
            var queues = new Queue<string>[chunks];
            for (var i = 0; i < chunks; i++)
            {
                readers[i] = new StreamReader(paths[i], Encoding.Unicode);
                queues[i] = new Queue<string>(bufferlen);
                LoadQueue(queues[i], readers[i], bufferlen);
            }

            // Merge!
            var sw = CreateStreamWriter(Path.Combine(SourceFolder, fileNameWithoutExt + "_sorted.txt"));
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
                        if (lowest_index < 0 ||
                            String.CompareOrdinal(
                                queues[j].Peek(), lowest_value) < 0)
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
                sw.WriteLine(lowest_value);

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