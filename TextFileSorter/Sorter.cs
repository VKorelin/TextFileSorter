using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextFileSorter
{
    public class Sorter
    {
        private static readonly char[] SplitChars = {'\r', '\n'};

        public Sorter()
        {
            
        }
        
        public void CreateSortedChunks(string fileName, Encoding encoding)
        {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var reader = new ChunkFileReader(fileName, encoding);
            ReadChunkResult nextChunk;
            var shift = 0;
            var chunkIdx = 0;
            do
            {
                nextChunk = reader.ReadNextChunk(shift);

                IEnumerable<string> lines = nextChunk.Chunk.Data.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
                if (!nextChunk.IsLastChunk && lines.Any())
                {
                    shift = - lines.Last().Length * 2;
                    lines = lines.SkipLast(1);
                }
                
                lines = lines
                    .Select(x => x.Split(". "))
                    .OrderBy(x => x[1])
                    .ThenBy(x => x[0])
                    .Select(x => $"{x[0]}. {x[1]}")
                    .ToArray();

                chunkIdx++;

                var savePath = Path.Combine(Program.ChunksFolder, $"{fileNameWithoutExt}_{chunkIdx}.txt");
                File.WriteAllLines(savePath, lines, encoding);
            } while (!nextChunk.IsLastChunk);

            reader.Dispose();
        }
    }
}