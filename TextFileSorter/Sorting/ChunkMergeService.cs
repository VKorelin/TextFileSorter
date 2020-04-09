using System;
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
            var chunksNumber = chunkNames.Count;
            var bufferSize = MaxUsage / chunksNumber;

            // ReSharper disable once PossibleLossOfFraction
            var bufferLength = (int) (bufferSize / RecordSize / RecordOverhead);

            var readers = new StreamReader[chunksNumber];
            var queues = new Queue<Entry>[chunksNumber];
            for (var i = 0; i < chunksNumber; i++)
            {
                readers[i] = new StreamReader(chunkNames[i], _configurationProvider.Encoding);
                queues[i] = new Queue<Entry>(bufferLength);
                LoadQueue(queues[i], readers[i], bufferLength);
            }

            Merge(chunksNumber, queues, readers, bufferLength, outputFileName);
            DeleteChunks(chunkNames, readers);
        }

        private void Merge(
            int chunksNumber, 
            IList<Queue<Entry>> queues, 
            IReadOnlyList<StreamReader> readers, 
            int bufferLen, 
            string outputFileName)
        {
            using var writer = new StreamWriter(outputFileName, false, _configurationProvider.Encoding);

            while (true)
            {
                var lowestIndex = -1;
                Entry lowestValue = null;
                for (var j = 0; j < chunksNumber; j++)
                {
                    if (queues[j] != null)
                    {
                        if (lowestIndex < 0 || queues[j].Peek().CompareTo(lowestValue) < 0)
                        {
                            lowestIndex = j;
                            lowestValue = queues[j].Peek();
                        }
                    }
                }

                if (lowestIndex == -1)
                {
                    break;
                }

                writer.WriteLine(lowestValue.ToString());
                queues[lowestIndex].Dequeue();
                
                if (queues[lowestIndex].Count == 0)
                {
                    LoadQueue(queues[lowestIndex], readers[lowestIndex], bufferLen);
                    
                    if (queues[lowestIndex].Count == 0)
                    {
                        queues[lowestIndex] = null;
                    }
                }
            }

            writer.Close();
        }

        private static void LoadQueue(Queue<Entry> queue, TextReader file, int records)
        {
            for (var i = 0; i < records; i++)
            {
                if (file.Peek() < 0) 
                    break;

                var entry = new Entry(file.ReadLine());
                queue.Enqueue(entry);
            }
        }

        private static void DeleteChunks(IList<string> chunkNames, IList<StreamReader> readers)
        {
            for (var i = 0; i < chunkNames.Count; i++)
            {
                readers[i].Close();
                File.Delete(chunkNames[i]);
            }
        }

        private class Entry : IComparable<Entry>
        {
            public Entry(string entry)
            {
                var entryArr = entry.Split(". ");
                
                Number = int.Parse(entryArr[0]);
                Line = entryArr[1];
            }
            
            public int Number { get; }
            
            public string Line { get; }

            public int CompareTo(Entry other)
            {
                if (other == null)
                {
                    return -1;
                }
                
                var lineCompareResult = string.CompareOrdinal(Line, other.Line);
                return lineCompareResult == 0 ? Number.CompareTo(other.Number) : lineCompareResult;
            }

            public override string ToString()
                => $"{Number}. {Line}";
        }
    }
}