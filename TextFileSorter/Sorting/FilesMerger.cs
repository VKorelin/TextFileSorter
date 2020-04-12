using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TextFileSorter.Configuration;

namespace TextFileSorter.Sorting
{
    public class FilesMerger : IFilesMerger
    {
        private const int MaxEntryLength = 100;
        
        private readonly IConfigurationProvider _configurationProvider;

        public FilesMerger(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }
        
        public string Merge(IList<string> filesToMerge, long ramAvailable)
        {
            var bytesPerFile = ramAvailable / filesToMerge.Count;
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

        private static string BuildOutputName(string sourceFile)
        {
            var dir = Path.GetDirectoryName(sourceFile);
            return Path.Combine(dir, Guid.NewGuid() + ".txt");
        }
    }
}